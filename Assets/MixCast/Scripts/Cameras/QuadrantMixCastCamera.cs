/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BlueprintReality.MixCast
{
    public class QuadrantMixCastCamera : MixCastCamera
    {
        public const string FIX_FG_SHADOW_SHADER = "Hidden/BPR/ScreenSpaceShadows NoFade";
        public const string ALPHA_OUT_SHADER = "Hidden/BPR/AlphaOut";
        public const string COPY_ALPHA_CHANNEL_SHADER = "Hidden/BPR/AlphaTransfer";

        public Vector3 playerHeadsetOffset = new Vector3(0, 0, -0.05f);
        public float layerOverlap = 0f;
        public Color clearColor = new Color(0, 0, 0, 0);
        public bool takeUnfilteredAlpha = true;

        private Material straightBlitMat;
        private Shader noForegroundShadowFadeShader;
        private Material alphaOutBlit;

        private Material postBlit;
        private RenderTexture quadrantTex;
        private RenderTexture fpTex;
        private RenderTexture lastFrameAlpha;

        CommandBuffer captureFP;

        private CommandBuffer grabAlphaCommand;
        private Material copyAlphaMat;

        protected override void Awake()
        {
            base.Awake();

            straightBlitMat = new Material(Shader.Find("Unlit/Texture"));

            noForegroundShadowFadeShader = Shader.Find(FIX_FG_SHADOW_SHADER);
            alphaOutBlit = new Material(Shader.Find(ALPHA_OUT_SHADER));
            postBlit = new Material(Shader.Find("Hidden/BPR/AlphaWrite"));

            grabAlphaCommand = new CommandBuffer();
            grabAlphaCommand.name = "Get Real Alpha";
            copyAlphaMat = new Material(Shader.Find(COPY_ALPHA_CHANNEL_SHADER));
        }

        protected override void BuildOutput()
        {
            base.BuildOutput();
            quadrantTex = new RenderTexture(Mathf.CeilToInt(Output.width * 0.5f), Mathf.CeilToInt(Output.height * 0.5f), 24, gameCamera.hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);

            //Capture the headset camera's output
            fpTex = new RenderTexture(quadrantTex.width, quadrantTex.height, 0);
            captureFP = new CommandBuffer();
            captureFP.Blit(BuiltinRenderTextureType.CameraTarget, fpTex);
            Camera.main.AddCommandBuffer(CameraEvent.AfterEverything, captureFP);

            if (takeUnfilteredAlpha)
            {
                lastFrameAlpha = new RenderTexture(Output.width, Output.height, 0);
                lastFrameAlpha.autoGenerateMips = lastFrameAlpha.useMipMap = false;
                lastFrameAlpha.Create();
                grabAlphaCommand.Blit(BuiltinRenderTextureType.CurrentActive, lastFrameAlpha);
            }
        }
        protected override void ReleaseOutput()
        {
            if (fpTex != null)
            {
                if( Camera.main != null )
                    Camera.main.RemoveCommandBuffer(CameraEvent.AfterEverything, captureFP);
                fpTex.Release();
                fpTex = null;
                captureFP.Release();
                captureFP = null;
            }

            if ( quadrantTex != null )
            {
                quadrantTex.Release();
                quadrantTex = null;
            }

            if (lastFrameAlpha != null)
            {
                lastFrameAlpha.Release();
                lastFrameAlpha = null;
                grabAlphaCommand.Clear();
            }

            base.ReleaseOutput();
        }
        void OnGUI()
        {
            //magic declaration that causes rendering to work
        }

        public override void RenderScene()
        {
            StartFrame();

            GL.LoadIdentity();
            ClearBuffer();

            //Reusing RT for background and foreground, so perform blit right after render
            RenderBackground();
            BlitBackground();

            RenderForeground();
            BlitForeground();

            //BlitFirstPerson();

            Graphics.Blit(null, postBlit);  //clear alpha
            Graphics.SetRenderTarget(null);

            CompleteFrame();
        }

        void ClearBuffer()
        {
            Graphics.SetRenderTarget(Output as RenderTexture);
            GL.Clear(true, true, clearColor);
        }
        void RenderBackground()
        {
            float oldNearClip = gameCamera.nearClipPlane;

            gameCamera.nearClipPlane = Mathf.Min(gameCamera.farClipPlane - 0.001f, CalculatePlayerDepth() - 0.5f * layerOverlap);

            RenderGameCamera(gameCamera, quadrantTex);

            gameCamera.nearClipPlane = oldNearClip;
        }
        void RenderForeground()
        {
            float oldFar = gameCamera.farClipPlane;
            CameraClearFlags oldFlags = gameCamera.clearFlags;
            Color oldBackgroundColor = gameCamera.backgroundColor;

            gameCamera.farClipPlane = Mathf.Max(gameCamera.nearClipPlane + 0.001f, CalculatePlayerDepth() + 0.5f * layerOverlap);

            gameCamera.clearFlags = CameraClearFlags.SolidColor;
            gameCamera.backgroundColor = new Color(clearColor.r, clearColor.g, clearColor.b, 0);

            Shader originalScreenSpaceShadowShader = GraphicsSettings.GetCustomShader(BuiltinShaderType.ScreenSpaceShadows);
            GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, noForegroundShadowFadeShader);

            if (takeUnfilteredAlpha)
                gameCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, grabAlphaCommand);  //Instruction to copy out the state of the RenderTexture before Image Effects are applied
            RenderGameCamera(gameCamera, quadrantTex);
            if (takeUnfilteredAlpha)
            {
                gameCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, grabAlphaCommand);
                Graphics.Blit(lastFrameAlpha, quadrantTex, copyAlphaMat);      //Overwrite the potentially broken post-effects alpha channel with the pre-effect copy
            }

            GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, originalScreenSpaceShadowShader);

            gameCamera.farClipPlane = oldFar;
            gameCamera.clearFlags = oldFlags;
            gameCamera.backgroundColor = oldBackgroundColor;
        }
        void BlitBackground()
        {
            bool oldSRGB = GL.sRGBWrite;
            GL.sRGBWrite = true;
            Graphics.SetRenderTarget(Output as RenderTexture);
            Graphics.DrawTexture(new Rect(0, Output.height * 0.5f, Output.width * 0.5f, Output.height * 0.5f), quadrantTex, straightBlitMat);
            GL.sRGBWrite = oldSRGB;
        }
        void BlitForeground()
        {
            bool oldSRGB = GL.sRGBWrite;
            GL.sRGBWrite = true;
            Graphics.SetRenderTarget(Output as RenderTexture);
            Graphics.DrawTexture(new Rect(0, 0, Output.width * 0.5f, Output.height * 0.5f), quadrantTex, straightBlitMat);
            Graphics.DrawTexture(new Rect(Output.width * 0.5f, 0, Output.width * 0.5f, Output.height * 0.5f), quadrantTex, alphaOutBlit);
            GL.sRGBWrite = oldSRGB;
        }

        void BlitFirstPerson()
        {
            Graphics.SetRenderTarget(Output as RenderTexture);
            Graphics.DrawTexture(new Rect(Output.width * 0.5f, Output.height * 0.5f, Output.width * 0.5f, Output.height * 0.5f), fpTex);
        }

        float CalculatePlayerDepth()
        {
            Vector3 playerPosition = Camera.main.transform.TransformPoint(playerHeadsetOffset);
            return Vector3.Dot(gameCamera.transform.forward, playerPosition - gameCamera.transform.position);
        }
    }
}