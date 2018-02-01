/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class SetActiveFromOutputMode : CameraComponent
    {
        public List<MixCastData.OutputMode> matchModes = new List<MixCastData.OutputMode>();

        public List<GameObject> activeIfMatch = new List<GameObject>();
        public List<GameObject> inactiveIfMatch = new List<GameObject>();

        private bool lastState;

        protected override void OnEnable()
        {
            base.OnEnable();
            SetNewState(CalculateNewState());
        }

        private void Update()
        {
            bool newState = CalculateNewState();
            if (newState != lastState)
                SetNewState(newState);
        }

        bool CalculateNewState()
        {
            return context.Data != null && matchModes.Contains(context.Data.outputMode);
        }
        void SetNewState(bool newState)
        {
            lastState = newState;
            activeIfMatch.ForEach(g => g.SetActive(newState));
            inactiveIfMatch.ForEach(g => g.SetActive(!newState));
        }
    }
}