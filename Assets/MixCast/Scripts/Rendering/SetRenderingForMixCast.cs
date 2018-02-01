/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class SetRenderingForMixCast : MonoBehaviour
    {
        public List<Renderer> targets = new List<Renderer>();
        [UnityEngine.Serialization.FormerlySerializedAs("renderForMixCast")]
        public bool renderForMixedReality = false;
        public bool renderForThirdPerson = false;

        protected virtual void OnEnable()
        {
            MixCastCamera.GameRenderStarted += HandleMixCastRenderStarted;
            MixCastCamera.GameRenderEnded += HandleMixCastRenderEnded;

            if (targets.Count == 0)
                GetComponentsInChildren<Renderer>(targets);

            //Set targets to the desired state during standard Unity rendering (not MixCast)
            targets.ForEach(r => r.enabled = !(renderForMixedReality || renderForThirdPerson));
        }
        protected virtual void OnDisable()
        {
            MixCastCamera.GameRenderStarted -= HandleMixCastRenderStarted;
            MixCastCamera.GameRenderEnded -= HandleMixCastRenderEnded;
        }


        private void HandleMixCastRenderStarted(MixCastCamera cam)
        {
            for( int i = 0; i < targets.Count; i++ )
            {
                targets[i].enabled = cam.ActiveFeeds.Count > 0 ? renderForMixedReality : renderForThirdPerson;
            }
        }
        private void HandleMixCastRenderEnded(MixCastCamera cam)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].enabled = !(cam.ActiveFeeds.Count > 0 ? renderForMixedReality : renderForThirdPerson);
            }
        }


        private void Reset()
        {
            targets.Clear();
            GetComponentsInChildren<Renderer>(targets);
        }
    }
}