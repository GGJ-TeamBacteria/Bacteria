/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    [RequireComponent(typeof(Toggle))]
    public class MixCastToggle : MonoBehaviour
    {
        private const string SETUP_URL = "https://blueprinttools.com/mixcast";

        public bool debugMode = false;
        public List<KeyCode> debugKeys = new List<KeyCode>() { KeyCode.LeftControl, KeyCode.RightControl };
        public List<GameObject> debugObjects = new List<GameObject>();

        public static bool CanToggleOn = true;

        private Toggle toggle;

        public GameObject inactiveImage;
        public GameObject activeImage;

        private void Start()
        {
            toggle = GetComponent<Toggle>();
            
            toggle.isOn = MixCast.Active;

            toggle.onValueChanged.AddListener(HandleToggleSet);
        }

        private void OnEnable()
        {
            foreach (GameObject go in debugObjects)
            {
                go.SetActive(debugMode);
            }
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(HandleToggleSet);
        }

        void HandleToggleSet(bool val)
        {
            if (val)
            {
                activeImage.SetActive(true);
                inactiveImage.SetActive(false);
            }
            else
            {
                activeImage.SetActive(false);
                inactiveImage.SetActive(true);
            }
            MixCast.SetActive(val);

            //Open MixCast webpage if the user doesn't have MixCast calibrated
            if( val && MixCast.Settings.cameras.Count == 0 )
            {
                Application.OpenURL(SETUP_URL);
            }

            if (debugMode)
            {
                bool triggerDebug = val && debugKeys.Exists(k => Input.GetKey(k));
                debugObjects.ForEach(g => g.SetActive(triggerDebug));
            } 
        }

        public void ToggleMixCast()
        {
            if (toggle.isOn)
                toggle.isOn = false;
            else
                toggle.isOn = CanToggleOn;
        }
    }
}