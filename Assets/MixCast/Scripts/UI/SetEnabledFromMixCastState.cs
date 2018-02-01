/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class SetEnabledFromMixCastState : MonoBehaviour {
        public List<MonoBehaviour> on = new List<MonoBehaviour>();
        public List<MonoBehaviour> off = new List<MonoBehaviour>();

        private bool lastState = false;

        private void OnEnable()
        {
            ApplyState(CalculateState());
        }
        private void Update()
        {
            bool newState = CalculateState();
            if (newState != lastState)
                ApplyState(newState);
        }

        bool CalculateState()
        {
            return MixCastCamera.ActiveCameras.Count > 0;
        }
        void ApplyState(bool state)
        {
            for (int i = 0; i < on.Count; i++)
                on[i].enabled = state;
            for (int i = 0; i < off.Count; i++)
                off[i].enabled = !state;

            lastState = state;
        }
    }
}