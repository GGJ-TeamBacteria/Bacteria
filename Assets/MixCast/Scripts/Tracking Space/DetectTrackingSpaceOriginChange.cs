/**********************************************************************************
* Blueprint Reality Inc. CONFIDENTIAL
* 2017 Blueprint Reality Inc.
* All Rights Reserved.
*
* NOTICE:  All information contained herein is, and remains, the property of
* Blueprint Reality Inc. and its suppliers, if any.  The intellectual and
* technical concepts contained herein are proprietary to Blueprint Reality Inc.
* and its suppliers and may be covered by Patents, pending patents, and are
* protected by trade secret or copyright law.
*
* Dissemination of this information or reproduction of this material is strictly
* forbidden unless prior written permission is obtained from Blueprint Reality Inc.
***********************************************************************************/

using UnityEngine;
using UnityEngine.VR;

namespace BlueprintReality.MixCast
{
    public class DetectTrackingSpaceOriginChange : MonoBehaviour
    {
        public enum ChangeHandle
        {
            ResetPositions = 0,
            OffsetPositions = 1,
        }

        public ChangeHandle handle;
        public bool saveToSettings = false;

        void Start()
        {
            if (UnityEngine.XR.XRDevice.isPresent)
            {
                var oldOrigin = MixCast.Settings.oculusOrigin;
                var newOrigin = TrackingSpaceOrigin.GetOriginOffsetData();

                //Debug.Log("Old: " + oldOrigin);
                //Debug.Log("New: " + newOrigin);

                if (WasOriginChanged(oldOrigin, newOrigin))
                {
                    Debug.Log("MixCast: Tracking Space Origin change detected! All cameras will reset to default positions.");

                    if (handle == ChangeHandle.ResetPositions)
                    {
                        ResetCamerasPosition();
                    }
                    else
                    {
                        ApplyNewOffsetToSettings(oldOrigin, newOrigin);
                    }

                    SaveSettings(newOrigin);
                }
            }
        }

        private bool WasOriginChanged(MixCastData.OculusOrigin oldOrigin, MixCastData.OculusOrigin newOrigin)
        {
            if (!oldOrigin.IsInitialized()
                || !newOrigin.IsInitialized())
            {
                return false;
            }

            return oldOrigin != newOrigin;
        }

        private void ResetCamerasPosition()
        {
            foreach (var camera in MixCast.Settings.cameras)
            {
                camera.worldPosition = MixCast.Settings.cameraStartPosition;
                camera.worldRotation = MixCast.Settings.cameraStartRotation;
            }
        }

        /// <summary>
        /// Experimental, does not work for now. Applying position and rotation offset to cameras does not match.
        /// </summary>
        private void ApplyNewOffsetToSettings(MixCastData.OculusOrigin oldOrigin, MixCastData.OculusOrigin newOrigin)
        {
            var diff = new MixCastData.OculusOrigin()
            {
                position = newOrigin.position - oldOrigin.position,
                rotation = newOrigin.rotation - oldOrigin.rotation
            };

            diff.position.z = -diff.position.z;

            foreach (var camera in MixCast.Settings.cameras)
            {
                camera.worldPosition -= diff.position;
                camera.worldRotation *= Quaternion.Euler(diff.rotation);
            }
        }

        private void SaveSettings(MixCastData.OculusOrigin newOrigin)
        {
            MixCast.Settings.oculusOrigin = newOrigin;

            if (saveToSettings)
            {
                MixCastData.WriteData();
            }
        }
    }
}