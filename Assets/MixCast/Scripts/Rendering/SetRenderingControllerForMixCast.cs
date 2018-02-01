/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class SetRenderingControllerForMixCast : SetRenderingForMixCast
    {
#if MIXCAST_STEAMVR
        private SteamVR_RenderModel render;

        protected override void OnEnable()
        {
            render = GetComponent<SteamVR_RenderModel>();

            SteamVR_Events.RenderModelLoaded.AddListener(HandleRenderLoaded);

            base.OnEnable();
        }

        private void HandleRenderLoaded(SteamVR_RenderModel curRender, bool success)
        {
            if (render != curRender)
                return;

            if (success)
                targets = new List<Renderer>(GetComponentsInChildren<Renderer>());
            else
                targets = new List<Renderer>();
        }

        protected override void OnDisable()
        {
            SteamVR_Events.RenderModelLoaded.RemoveListener(HandleRenderLoaded);

            base.OnDisable();
        }
#endif


    }
}