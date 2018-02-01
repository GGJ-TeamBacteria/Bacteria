using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.Text
{
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class SetTextFromLocalization : MonoBehaviour
    {
        public string id;
        public bool toUpper = false;

        protected void OnEnable()
        {
            Localization.LanguageChanged += RefreshText;
            RefreshText();
        }
        protected void OnDisable()
        {
            Localization.LanguageChanged -= RefreshText;
        }

        void RefreshText()
        {
            string text = Localization.Get(id);
            if (toUpper)
                text = text.ToUpper();
            GetComponent<UnityEngine.UI.Text>().text = text;
        }
    }
}