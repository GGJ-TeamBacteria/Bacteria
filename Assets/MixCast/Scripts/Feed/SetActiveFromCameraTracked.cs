/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class SetActiveFromCameraTracked : CameraComponent
    {
        public List<GameObject> tracked = new List<GameObject>();
        public List<GameObject> untracked = new List<GameObject>();

        private bool lastState;

        protected override void OnEnable()
        {
            base.OnEnable();
     
            SetState(CalculateNewState());
        }
        void Update()
        {
            bool newState = CalculateNewState();
            if (newState != lastState)
                SetState(newState);
        }

        bool CalculateNewState()
        {
            if (context.Data == null || context.Data.wasTracked == false)
                return false;

#if MIXCAST_STEAMVR
            if (VRInfo.IsDeviceOpenVR() && IsTracked_Steam())
                return true;
#endif
#if MIXCAST_OCULUS
            if (VRInfo.IsDeviceOculus() && IsTracked_Oculus())
                return true;
#endif

            return false;
        }
        void SetState(bool newState)
        {
            tracked.ForEach(g => g.SetActive(newState));
            untracked.ForEach(g => g.SetActive(!newState));
            lastState = newState;
        }


#if MIXCAST_STEAMVR
        private SteamVR_TrackedObject.EIndex trackedByIndexSteam = SteamVR_TrackedObject.EIndex.None;

        bool IsTracked_Steam()
        {
            if (string.IsNullOrEmpty(context.Data.trackedByDevice))
                return false;

            try {
                if (trackedByIndexSteam.ToString() != context.Data.trackedByDevice)
                    trackedByIndexSteam = (SteamVR_TrackedObject.EIndex)System.Enum.Parse(typeof(SteamVR_TrackedObject.EIndex), context.Data.trackedByDevice);

                if (trackedByIndexSteam == SteamVR_TrackedObject.EIndex.None || (int)trackedByIndexSteam >= Valve.VR.OpenVR.k_unMaxTrackedDeviceCount)
                    return false;

                SteamVR_Controller.Device dev = SteamVR_Controller.Input((int)trackedByIndexSteam);
                return dev.valid && dev.hasTracking;
            }
            catch(System.Exception)
            {
                return false;
            }
        }
#endif

#if MIXCAST_OCULUS
        bool IsTracked_Oculus()
        {
            if (string.IsNullOrEmpty(context.Data.trackedByDevice))
                return false;

            try {
                OVRInput.Controller controller = OVRInput.Controller.None;
                if (context.Data.trackedByDevice == "Device1")
                    controller = OVRInput.Controller.LTouch;
                else if (context.Data.trackedByDevice == "Device2")
                    controller = OVRInput.Controller.RTouch;

                return OVRInput.IsControllerConnected(controller) && OVRInput.GetControllerPositionTracked(controller) && OVRInput.GetControllerOrientationTracked(controller);
            }
            catch (System.Exception)
            {
                return false;
            }
        }
#endif
    }
}