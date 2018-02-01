/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    public class SetGraphicAlphaFromMixCastSetting : MonoBehaviour
    {
        private Graphic graphic;

        private void Awake()
        {
            graphic = GetComponent<Graphic>();
            Update();
        }

        private void Update()
        {
            Color c = graphic.color;
            c.a = MixCast.Settings.sceneDisplay.alpha;
            graphic.color = c;
        }
    }
}