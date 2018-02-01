/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class SetTransformFromCameraSettings : CameraComponent
    {
        protected Vector3 smoothPositionVel, smoothRotationVel;

        protected override void OnEnable()
        {
            base.OnEnable();

            TrackedDeviceManager.OnTransformsUpdated += UpdateTransform;

            HandleDataChanged();
        }
        protected override void OnDisable()
        {
            TrackedDeviceManager.OnTransformsUpdated -= UpdateTransform;

            base.OnDisable();
        }

        protected override void HandleDataChanged()
        {
            base.HandleDataChanged();

            UpdateTransform();
        }

        private void Update()
        {
            UpdateTransform();
        }

        protected virtual void UpdateTransform()
        {
            if (context.Data != null)
            {
                Vector3 pos;
                Quaternion rot;
                GetCameraTransform(out pos, out rot);
                SetTransformFromTarget(pos, rot);
            }
        }

        public void GetCameraTransform(out Vector3 position, out Quaternion rotation)
        {
            if (context.Data.wasTracked && GetTrackingTransform(out position, out rotation))
            {
                position = position + rotation * context.Data.trackedPosition;
                rotation = rotation * context.Data.trackedRotation;
            }
            else
            {
                position = context.Data.worldPosition;
                rotation = context.Data.worldRotation;
            }
        }

        void SetTransformFromTarget(Vector3 newPos, Quaternion newRot, bool instant = false)
        {
            if (context.Data.positionSmoothTime > 0 && !instant)
                newPos = Vector3.SmoothDamp(transform.localPosition, newPos, ref smoothPositionVel, context.Data.positionSmoothTime);
            transform.localPosition = newPos;

            if (context.Data.rotationSmoothTime > 0 && !instant)
            {
                Vector3 curEuler = transform.localEulerAngles;
                Vector3 newEuler = newRot.eulerAngles;
                newRot = Quaternion.Euler(new Vector3(
                    Mathf.SmoothDampAngle(curEuler.x, newEuler.x, ref smoothRotationVel.x, context.Data.rotationSmoothTime),
                    Mathf.SmoothDampAngle(curEuler.y, newEuler.y, ref smoothRotationVel.y, context.Data.rotationSmoothTime),
                    Mathf.SmoothDampAngle(curEuler.z, newEuler.z, ref smoothRotationVel.z, context.Data.rotationSmoothTime)
                    ));
            }
            transform.localRotation = newRot;
        }

        public bool GetTrackingTransform(out Vector3 position, out Quaternion rotation)
        {
            if (!string.IsNullOrEmpty(context.Data.trackedByDeviceId))
            {
                if (TrackedDeviceManager.Instance.GetDeviceTransformByGuid(context.Data.trackedByDeviceId, out position, out rotation))
                    return true;
            }

#if MIXCAST_STEAMVR
            if (VRInfo.IsDeviceOpenVR())
            {
                //Fall back to index if guid isn't found
                SteamVR_TrackedObject.EIndex deviceIndex = (SteamVR_TrackedObject.EIndex)System.Enum.Parse(typeof(SteamVR_TrackedObject.EIndex), context.Data.trackedByDevice);
                return TrackedDeviceManager.Instance.GetDeviceTransformByIndex((int)deviceIndex, out position, out rotation);
            }
#endif
#if MIXCAST_OCULUS
            if(VRInfo.IsDeviceOculus())
            {
                switch(context.Data.trackedByDevice)
                {
                    case "Hmd":
                        return TrackedDeviceManager.Instance.GetDeviceTransformByRole(TrackedDeviceManager.DeviceRole.Head, out position, out rotation);
                    case "Device1":
                        return TrackedDeviceManager.Instance.GetDeviceTransformByRole(TrackedDeviceManager.DeviceRole.LeftHand, out position, out rotation);
                    case "Device2":
                        return TrackedDeviceManager.Instance.GetDeviceTransformByRole(TrackedDeviceManager.DeviceRole.RightHand, out position, out rotation);
                    default:
                        break;
                }
            }
#endif

            position = Vector3.zero;
            rotation = Quaternion.identity;
            return false;
        }
    }
}