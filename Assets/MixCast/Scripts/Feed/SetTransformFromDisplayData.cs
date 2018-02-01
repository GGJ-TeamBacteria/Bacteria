/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class SetTransformFromDisplayData : CameraComponent
    {
        protected virtual void Update()
        {
            if (context.Data == null)
                return;

            //transform.localScale = context.Data.displayData.scale * Vector3.one;
            switch (context.Data.displayData.mode)
            {
                case MixCastData.SceneDisplayData.PlacementMode.Camera:
                    MixCastCamera target = MixCastCamera.ActiveCameras.Find(c => c.context.Data == context.Data);
                    if (target != null)
                    {
                        transform.position = target.displayTransform.position;
                        transform.rotation = target.displayTransform.rotation;
                    }
                    
                    break;
                case MixCastData.SceneDisplayData.PlacementMode.World:
                    transform.localPosition = context.Data.displayData.position;
                    transform.localRotation = context.Data.displayData.rotation;
                    break;
                case MixCastData.SceneDisplayData.PlacementMode.Headset:
                    break;
                default:
                    break;
            }
        }
    }
}