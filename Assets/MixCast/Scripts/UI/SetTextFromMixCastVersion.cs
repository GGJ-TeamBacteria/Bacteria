/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class SetTextFromMixCastVersion : MonoBehaviour
    {
        public string formatStr = "{0}";

        private void Awake()
        {
            GetComponent<UnityEngine.UI.Text>().text = string.Format(formatStr, MixCast.VERSION_STRING);
        }
    }
}