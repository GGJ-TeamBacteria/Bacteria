/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class ApplyStaticSubtractionToInputFeed : CameraComponent
    {
        private const string SHADER_KEYWORD = "BG_REMOVAL_STATIC";

        public InputFeed feed;

        public string midMapProperty = "_KeyMidTex";
        public string rangeMapProperty = "_KeyRangeTex";
        public string chromaKeyFeathersParameter = "_KeyHsvFeathering";

        protected override void OnEnable()
        {
            if (feed == null)
                feed = GetComponentInParent<InputFeed>();

            base.OnEnable();

            feed.OnRenderStarted += StartRender;
            feed.OnRenderEnded += StopRender;
        }
        protected override void OnDisable()
        {
            feed.OnRenderStarted -= StartRender;
            feed.OnRenderEnded -= StopRender;

            base.OnDisable();
        }

        protected void StartRender()
        {
            Material blitMaterial = feed.blitMaterial;
            if( blitMaterial != null && context.Data != null )
            {
                bool staticSubtractionActive = context.Data.isolationMode == MixCastData.IsolationMode.StaticSubtraction && context.Data.staticSubtractionData.active;
                if (staticSubtractionActive)
                    blitMaterial.EnableKeyword(SHADER_KEYWORD);

                //if (blitMaterial.HasProperty(midMapProperty))
                    blitMaterial.SetTexture(midMapProperty, staticSubtractionActive ? context.Data.staticSubtractionData.midValueTexture.Tex : Texture2D.blackTexture);
                //if (blitMaterial.HasProperty(rangeMapProperty))
                    blitMaterial.SetTexture(rangeMapProperty, staticSubtractionActive ? context.Data.staticSubtractionData.rangeValueTexture.Tex : Texture2D.blackTexture);

                if (!string.IsNullOrEmpty(chromaKeyFeathersParameter))
                    blitMaterial.SetVector(chromaKeyFeathersParameter, staticSubtractionActive ? context.Data.staticSubtractionData.keyHsvFeathering : Vector3.zero);
            }
        }

        protected void StopRender()
        {
            Material blitMaterial = feed.blitMaterial;
            if (blitMaterial != null && context.Data != null)
            {
                bool staticSubtractionActive = context.Data.isolationMode == MixCastData.IsolationMode.StaticSubtraction && context.Data.staticSubtractionData.active;
                if (staticSubtractionActive)
                    blitMaterial.DisableKeyword(SHADER_KEYWORD);
            }
        }
    }
}