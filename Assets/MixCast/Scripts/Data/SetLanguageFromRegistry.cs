using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class SetLanguageFromRegistry : MonoBehaviour
    {
        void Awake()
        {
            Text.Localization.CurrentLanguage = MixCast.Settings.language;
        }
    }
}
