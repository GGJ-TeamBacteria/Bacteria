/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace BlueprintReality.MixCast
{
    public class SlateUi : MonoBehaviour
    {
        [Header("UI")]
        public GameObject root;
        public List<UnityEngine.UI.Text> texts = new List<UnityEngine.UI.Text>();
        public string indexFormat = "{0}";

        [Header("Controls")]
        public KeyCode toggleKey = KeyCode.BackQuote;
        public KeyCode incrementKey = KeyCode.UpArrow;
        public KeyCode decrementKey = KeyCode.DownArrow;

        [Header("Saving")]
        public string playerPrefKey = "slate";


        private int takeIndex = 0;

        void Start()
        {
            if (!string.IsNullOrEmpty(playerPrefKey))
                takeIndex = PlayerPrefs.GetInt(playerPrefKey);

            root.SetActive(false);
        }

        void OnDestroy()
        {
            if (!string.IsNullOrEmpty(playerPrefKey))
                PlayerPrefs.SetInt(playerPrefKey, takeIndex);
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                if (root.activeSelf)
                    root.SetActive(false);
                else
                {
                    takeIndex++;
                    SetTexts();
                    root.SetActive(true);
                }
            }
            else if (root.activeSelf)
            {
                if (Input.GetKeyDown(incrementKey))
                {
                    takeIndex++;
                    SetTexts();
                }
                else if (Input.GetKeyDown(decrementKey))
                {
                    takeIndex = Mathf.Max(0, takeIndex - 1);
                    SetTexts();
                }
            }
        }

        void SetTexts()
        {
            string val = string.Format(indexFormat, takeIndex);
            texts.ForEach(t => t.text = val);
        }
    }
}
