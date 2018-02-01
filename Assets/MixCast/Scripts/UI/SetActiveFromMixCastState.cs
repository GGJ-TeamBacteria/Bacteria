/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class SetActiveFromMixCastState : MonoBehaviour {
        public List<GameObject> on = new List<GameObject>();
        public List<GameObject> off = new List<GameObject>();

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
                on[i].SetActive(state);
            for (int i = 0; i < off.Count; i++)
                off[i].SetActive(!state);

            lastState = state;
        }
    }
}