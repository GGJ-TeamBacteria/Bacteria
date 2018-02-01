/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    public class SetTextFromFPS : MonoBehaviour
    {
        public float updateFrequency = 0.5f;

        public string formatStr = "{00:0}";

        private void OnEnable()
        {
            StartCoroutine(Run());
        }
        private void OnDisable()
        {
            StopCoroutine("Run");
        }


        IEnumerator Run()
        {
            UnityEngine.UI.Text text = GetComponent<UnityEngine.UI.Text>();
            while(true)
            {
                int lastFrameCount = Time.frameCount;
                float lastTime = Time.unscaledTime;
                yield return new WaitForSecondsRealtime(updateFrequency);
                int framesElapsed = Time.frameCount - lastFrameCount;
                float timeElapsed = Time.unscaledTime - lastTime;
                text.text = string.Format(formatStr, (float)framesElapsed / timeElapsed);
            }
        }
    }
}