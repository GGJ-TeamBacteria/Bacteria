/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    public class SetScaleFromMixCastSetting : CameraComponent
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        
            Update();
        }

        private void Update()
        {
            if (context.Data == null)
                return;

            transform.localScale = context.Data.displayData.scale * Vector3.one;
        }
    }
}