/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;
using UnityEngine.Rendering;

namespace BlueprintReality.MixCast
{
    public class ApplyChromakeyToInputFeed : CameraComponent
    {
        private const string SHADER_KEYWORD = "BG_REMOVAL_CHROMA";

        public const string CHROMA_DESAT_BAND_WIDTH_PARAM = "_KeyDesaturateBandWidth";
        public const string CHROMA_DESAT_FALLOFF_WIDTH_PARAM = "_KeyDesaturateFalloffWidth";

        public InputFeed feed;

        public string chromaKeyColorParameter = "_KeyHsvMid";
        public string chromaKeyLimitsParameter = "_KeyHsvRange";
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
            if (blitMaterial != null)
            {
                if (context.Data != null)
                {
                    bool chromakeyActive = context.Data.isolationMode == MixCastData.IsolationMode.Chromakey && context.Data.chromakeying.active;

                    if (chromakeyActive)
                        blitMaterial.EnableKeyword(SHADER_KEYWORD);

                    if (!string.IsNullOrEmpty(chromaKeyColorParameter))
                        blitMaterial.SetVector(chromaKeyColorParameter, chromakeyActive ? context.Data.chromakeying.keyHsvMid : Vector3.zero);
                    if (!string.IsNullOrEmpty(chromaKeyLimitsParameter))
                        blitMaterial.SetVector(chromaKeyLimitsParameter, chromakeyActive ? context.Data.chromakeying.keyHsvRange : Vector3.zero);
                    if (!string.IsNullOrEmpty(chromaKeyFeathersParameter))
                        blitMaterial.SetVector(chromaKeyFeathersParameter, chromakeyActive ? context.Data.chromakeying.keyHsvFeathering : Vector3.zero);
                    blitMaterial.SetFloat(CHROMA_DESAT_BAND_WIDTH_PARAM, chromakeyActive ? context.Data.chromakeying.keyDesaturationBandWidth : 1f);
                    blitMaterial.SetFloat(CHROMA_DESAT_FALLOFF_WIDTH_PARAM, chromakeyActive ? context.Data.chromakeying.keyDesaturationFalloffWidth : 0f);
                }
            }
        }
        protected void StopRender()
        {
            Material blitMaterial = feed.blitMaterial;
            if (blitMaterial != null && context.Data != null)
            {
                bool chromakeyActive = context.Data.isolationMode == MixCastData.IsolationMode.Chromakey && context.Data.chromakeying.active;
                if (chromakeyActive)
                    blitMaterial.DisableKeyword(SHADER_KEYWORD);
            }
        }
    }
}