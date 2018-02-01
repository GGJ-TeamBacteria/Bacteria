/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;

namespace BlueprintReality.MixCast
{
    /// <summary>
    /// MixCast Studio is using SteamVR (with OpenVR) as the main VR SDK.
    /// But Oculus SDK have different point of origin in the real world than SteamVR.
    /// They can be setup independently.
    ///
    /// When user of MixCast SDK uses Oculus as its main VR SDK for their project, the virtual camera
    /// is wronly positioned.
    /// 
    /// This script sets origin offset to it's GameObject transform.
    /// </summary>
    public class SetOculusOriginOffsetFromSettings : MonoBehaviour
    {
        void Start()
        {
            if (VRInfo.IsDeviceOculus())
            {
                OffsetTransform();
            }
        }
        
        /// <summary>
        /// Moves and Rotates this GameObject by difference between SteamVR and Oculus origins.
        /// </summary>
        private void OffsetTransform()
        {
            if (MixCast.Settings.oculusOrigin != null)
            {
                // Remove difference between height of Oculus and SteamVR.
                var pos = MixCast.Settings.oculusOrigin.position;

#if MIXCAST_OCULUS
                pos.y += OVRPlugin.eyeHeight;
#endif
                
                transform.Rotate(MixCast.Settings.oculusOrigin.rotation);
                transform.Translate(Vector3.Scale(pos, transform.lossyScale));
            }
        }
    }
}