using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    public class SetBufferDelayFromShortcutKey : CameraComponent
    {
        public float minVal = 1f;
        public float maxVal = 5000f;

        public float step = 50f;
        public float multiplier = 0.001f;
        
        public KeyCode goUpKey = KeyCode.RightBracket;
        public KeyCode goUpModifier = KeyCode.None;
               
        public KeyCode goDownKey = KeyCode.LeftBracket;
        public KeyCode goDownModifier = KeyCode.None;

        public KeyCode excludedKey1 = KeyCode.LeftControl;
        public KeyCode excludedKey2 = KeyCode.RightControl;

        void Update()
        {
            if (Selectable.allSelectables.Find(s => s is InputField && (s as InputField).isFocused) != null)
            {
                return;
            }

            bool goUp = Input.GetKeyDown(goUpKey);
            if (goUpModifier != KeyCode.None)
                goUp &= Input.GetKey(goUpModifier);

            bool goDown = Input.GetKeyDown(goDownKey);
            if (goDownModifier != KeyCode.None)
                goDown &= Input.GetKey(goDownModifier);
            
            if (goUp && !Input.GetKey(excludedKey1) && !Input.GetKey(excludedKey2))
            {
                UpdateValue(step);
                Debug.Log("Increase buffer timing to " + context.Data.bufferTime.ToString());
            }
            else if (goDown && !Input.GetKey(excludedKey1) && !Input.GetKey(excludedKey2))
            {
                UpdateValue(-step);
                Debug.Log("Decrease buffer timing " + context.Data.bufferTime.ToString());
            }
        }

        private void UpdateValue(float delta)
        {
            context.Data.bufferTime = Mathf.Clamp(context.Data.bufferTime / multiplier + delta, minVal, maxVal) * multiplier;
        }
    }
}