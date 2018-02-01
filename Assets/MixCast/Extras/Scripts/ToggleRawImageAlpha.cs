using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{

    public class ToggleRawImageAlpha : MonoBehaviour {

        public RawImage objectToToggle;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void Toggle()
        {
            if (objectToToggle.gameObject.activeSelf)
            {
                objectToToggle.color = new Color(objectToToggle.color.r, objectToToggle.color.g, objectToToggle.color.b, 0);
                //objectToToggle.gameObject.SetActive(false);
            }
            else
            {
                objectToToggle.color = new Color(objectToToggle.color.r, objectToToggle.color.g, objectToToggle.color.b, 1);
                //objectToToggle.gameObject.SetActive(true);
            }
        }

        public void ToggleIfMixCast()
        {
            if (MixCast.Active)
            {
                Toggle();
            }
        }
    }
}
