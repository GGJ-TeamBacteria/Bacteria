/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class SetCameraParametersFromMainCamera : MonoBehaviour
    {
        public bool clearSettings = true;
        public bool cullingMask = true;
        public bool clippingPlanes = true;
        public bool hdr = true;

        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            LateUpdate();
        }
        private void LateUpdate()
        {
            if (Camera.main == null)
                return;

            if (clearSettings)
            {
                cam.clearFlags = Camera.main.clearFlags;
                cam.backgroundColor = Camera.main.backgroundColor;
            }
            if( cullingMask)
            {
                cam.cullingMask = Camera.main.cullingMask;
            }
            if (clippingPlanes)
            {
                cam.nearClipPlane = Camera.main.nearClipPlane;
                cam.farClipPlane = Camera.main.farClipPlane;
            }
            if( hdr )
            {
#if UNITY_5_6_OR_NEWER
                cam.allowHDR = Camera.main.allowHDR;
#else
                cam.hdr = Camera.main.hdr;
#endif
            }
        }
    }
}