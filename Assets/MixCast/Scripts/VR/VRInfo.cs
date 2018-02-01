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

using UnityEngine.VR;

namespace BlueprintReality.MixCast
{
    public class VRInfo
    {
        public static string loadedDeviceNameOverride = "";

        public static bool IsDeviceOculus()
        {
            if (!string.IsNullOrEmpty(loadedDeviceNameOverride))
                return loadedDeviceNameOverride == "Oculus";
            return UnityEngine.XR.XRSettings.loadedDeviceName == "Oculus";
        }

        public static bool IsDeviceOpenVR()
        {
            if (!string.IsNullOrEmpty(loadedDeviceNameOverride))
                return loadedDeviceNameOverride == "OpenVR";
            return UnityEngine.XR.XRSettings.loadedDeviceName == "OpenVR";
        }

        public static bool IsVRModelVive()
        {
            return UnityEngine.XR.XRDevice.model.ToLower().Contains("vive");
        }

        public static bool IsVRModelOculus()
        {
            return UnityEngine.XR.XRDevice.model.ToLower().Contains("rift");
        }

        public static string GetDeviceSerial(uint deviceIndex)
        {
#if MIXCAST_STEAMVR
            if (!IsDeviceOpenVR())
            {
                return deviceIndex.ToString();
            }

            var error = Valve.VR.ETrackedPropertyError.TrackedProp_Success;
            var capactiy = Valve.VR.OpenVR.System.GetStringTrackedDeviceProperty((uint)deviceIndex, Valve.VR.ETrackedDeviceProperty.Prop_SerialNumber_String, null, 0, ref error);
            if (capactiy > 1)
            {
                var result = new System.Text.StringBuilder((int)capactiy);
                Valve.VR.OpenVR.System.GetStringTrackedDeviceProperty((uint)deviceIndex, Valve.VR.ETrackedDeviceProperty.Prop_SerialNumber_String, result, capactiy, ref error);
                return result.ToString();
            }

            return deviceIndex.ToString();
#else
            // TODO: Find a way how to get Device Serial Number from Oculus SDK
            return deviceIndex.ToString();
#endif
        }
    }
}
