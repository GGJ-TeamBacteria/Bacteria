/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BlueprintReality.MixCast
{
    //Place at the root of the camera hierarchy. 
    public class BufferedMixCastCamera : MixCastCamera
    {
        public const string DEPTH_WRITE_SHADER = "Hidden/BPR/DepthWrite";
        public const string FIX_FG_SHADOW_SHADER = "Hidden/BPR/ScreenSpaceShadows NoFade";
        public const string BLIT_BACKGROUND_SHADER = "Hidden/BPR/Background Blit";
        public const string BLIT_FOREGROUND_SHADER = "Hidden/BPR/Foreground Blit";
        public const string BLIT_FOREGROUND_PMA_SHADER = "Hidden/BPR/Foreground Blit PMA";
        public const string COPY_ALPHA_CHANNEL_SHADER = "Hidden/BPR/AlphaTransfer";

        [System.Serializable]
        public class GameFrame
        {
            public float playerDist;

            public RenderTexture foregroundBuffer;
            public RenderTexture backgroundBuffer;

            public Matrix4x4 cameraToWorld;
            public Matrix4x4 cameraProjection;
            
        }

        public float layerOverlap = 0f;
        public LayerMask forceToBackground = 0;
        public bool takeUnfilteredAlpha = true;
        public bool premultipliedAlpha = false;

        public event System.Action BackgroundRendered;

        private FrameDelayQueue<GameFrame> frames;
        private RenderTexture lastFrameAlpha;

        private Shader blockoutBackgroundShader;
        private Shader noForegroundShadowFadeShader;

        private Material blitBackgroundMat;
        private Material blitForegroundMat;
        private Material postBlit;

        private CommandBuffer grabAlphaCommand;
        private Material copyAlphaMat;

        private Camera cleanProxyCamera;
        private CommandBuffer finalizeFrameCommand;

        protected override void Awake()
        {
            base.Awake();
        
            blockoutBackgroundShader = Shader.Find(DEPTH_WRITE_SHADER);
            noForegroundShadowFadeShader = Shader.Find(FIX_FG_SHADOW_SHADER);
            blitBackgroundMat = new Material(Shader.Find(BLIT_BACKGROUND_SHADER));
            if (!premultipliedAlpha)
                blitForegroundMat = new Material(Shader.Find(BLIT_FOREGROUND_SHADER));
            else
                blitForegroundMat = new Material(Shader.Find(BLIT_FOREGROUND_PMA_SHADER));
            postBlit = new Material(Shader.Find("Hidden/BPR/AlphaWrite"));

            grabAlphaCommand = new CommandBuffer();
            grabAlphaCommand.name = "Get Real Alpha";
            copyAlphaMat = new Material(Shader.Find(COPY_ALPHA_CHANNEL_SHADER));

            GameObject cleanProxyObj = new GameObject("Clean Camera");
            cleanProxyObj.hideFlags = HideFlags.HideAndDontSave;
            cleanProxyObj.transform.SetParent(gameCamera.transform);
            cleanProxyObj.transform.localPosition = Vector3.zero;
            cleanProxyObj.transform.localRotation = Quaternion.identity;
            cleanProxyObj.transform.localScale = Vector3.one;
            cleanProxyCamera = cleanProxyObj.AddComponent<Camera>();
            cleanProxyCamera.enabled = false;

            finalizeFrameCommand = new CommandBuffer();
            finalizeFrameCommand.name = "Finalize Frame";
        }

        protected override void BuildOutput()
        {
            base.BuildOutput();

            frames = new FrameDelayQueue<GameFrame>();
            frames.delayDuration = context.Data.bufferTime;
            frames.AllocateFrames();

            if (takeUnfilteredAlpha)
            {
                lastFrameAlpha = new RenderTexture(Output.width, Output.height, 0);
                lastFrameAlpha.autoGenerateMips = lastFrameAlpha.useMipMap = false;
                lastFrameAlpha.Create();
                grabAlphaCommand.Blit(BuiltinRenderTextureType.CurrentActive, lastFrameAlpha);
            }

            finalizeFrameCommand.Blit(Output as RenderTexture, BuiltinRenderTextureType.CurrentActive);
        }
        protected override void ReleaseOutput()
        {
            frames.unusedFrames.ForEach(f =>
            {
                if(f.data != null && f.data.backgroundBuffer != null )
                    f.data.backgroundBuffer.Release();
                if (f.data != null && f.data.foregroundBuffer != null)
                    f.data.foregroundBuffer.Release();
            });
            frames.usedFrames.ForEach(f =>
            {
                if (f.data != null && f.data.backgroundBuffer != null)
                    f.data.backgroundBuffer.Release();
                if (f.data != null && f.data.foregroundBuffer != null)
                    f.data.foregroundBuffer.Release();
            });
            frames = null;

            if (lastFrameAlpha != null)
            {
                lastFrameAlpha.Release();
                lastFrameAlpha = null;
                grabAlphaCommand.Clear();
            }

            finalizeFrameCommand.Clear();

            base.ReleaseOutput();
        }

        void OnGUI()
        {
            //magic declaration that causes rendering to work
        }


        public override void RenderScene()
        {
            StartFrame();

            frames.delayDuration = context.Data.bufferTime;
            frames.Update();

            GameFrame newFrame = PrepareNewFrame();

            newFrame.playerDist = gameCamera.transform.TransformVector(Vector3.forward).magnitude * CalculatePlayerDistance();  //Scale distance by camera scale
            newFrame.cameraToWorld = cleanProxyCamera.cameraToWorldMatrix;
            newFrame.cameraProjection = cleanProxyCamera.projectionMatrix;

            cleanProxyCamera.CopyFrom(gameCamera);
            RenderBackground(newFrame);
            RenderForeground(newFrame);

            GameFrame bufferedFrame = frames.OldestFrameData;

            GL.LoadIdentity();
            ClearBuffer();
            BlitBackground(bufferedFrame);
            if (BackgroundRendered != null)
                BackgroundRendered();

            //Update shader properties for Feeds to access
            Shader.SetGlobalFloat("_CamNear", cleanProxyCamera.nearClipPlane);
            Shader.SetGlobalFloat("_CamFar", cleanProxyCamera.farClipPlane);
            Shader.SetGlobalMatrix("_WorldToCam", bufferedFrame.cameraToWorld.inverse);
            Shader.SetGlobalMatrix("_CamToWorld", bufferedFrame.cameraToWorld);
            Shader.SetGlobalMatrix("_CamProjection", bufferedFrame.cameraProjection);

            for (int i = 0; i < ActiveFeeds.Count; i++)
                BlitFeed(ActiveFeeds[i]);

            if( bufferedFrame.playerDist > 0 )
                BlitForeground(bufferedFrame);

            Graphics.SetRenderTarget(Output as RenderTexture);
            Graphics.Blit(null, postBlit);
            Graphics.SetRenderTarget(null);

            RenderFinalPass();

            CompleteFrame();
        }

        //Gets an empty frame from the pool and ensures its ready to be filled
        GameFrame PrepareNewFrame()
        {
            FrameDelayQueue<GameFrame>.Frame<GameFrame> newFrame = frames.GetNewFrame();
            if (newFrame.data == null)
                newFrame.data = new GameFrame();

            if (newFrame.data.backgroundBuffer != null && (newFrame.data.backgroundBuffer.width != Output.width || newFrame.data.backgroundBuffer.height != Output.height))
            {
                newFrame.data.backgroundBuffer.Release();
                newFrame.data.backgroundBuffer = null;
            }
            if (newFrame.data.backgroundBuffer == null)
                newFrame.data.backgroundBuffer = new RenderTexture(Output.width, Output.height, 24, gameCamera.hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);

            if (newFrame.data.foregroundBuffer != null && (newFrame.data.foregroundBuffer.width != Output.width || newFrame.data.foregroundBuffer.height != Output.height))
            {
                newFrame.data.foregroundBuffer.Release();
                newFrame.data.foregroundBuffer = null;
            }
            if (newFrame.data.foregroundBuffer == null)
                newFrame.data.foregroundBuffer = new RenderTexture(Output.width, Output.height, 24, gameCamera.hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);

            return newFrame.data;
        }

        void ClearBuffer()
        {
            Graphics.SetRenderTarget(Output as RenderTexture);
            GL.Clear(true, true, Color.black);
            //Graphics.SetRenderTarget(null); 
        }

        void RenderBackground(GameFrame targetFrame)
        {
            float oldNearClip = cleanProxyCamera.nearClipPlane;
            
            cleanProxyCamera.nearClipPlane = Mathf.Clamp(targetFrame.playerDist - 0.5f * layerOverlap, 0.001f, cleanProxyCamera.farClipPlane - 0.001f);

            RenderGameCamera(cleanProxyCamera, targetFrame.backgroundBuffer);

            if( forceToBackground > 0 )
            {
                float oldFarClip = cleanProxyCamera.farClipPlane;
                LayerMask oldCull = cleanProxyCamera.cullingMask;
                CameraClearFlags oldClear = cleanProxyCamera.clearFlags;

                cleanProxyCamera.farClipPlane = targetFrame.playerDist + 0.5f * layerOverlap;
                cleanProxyCamera.nearClipPlane = oldNearClip;
                cleanProxyCamera.cullingMask = forceToBackground;
                cleanProxyCamera.clearFlags = CameraClearFlags.Depth;

                RenderGameCamera(cleanProxyCamera, targetFrame.backgroundBuffer);

                cleanProxyCamera.cullingMask = oldCull;
                cleanProxyCamera.farClipPlane = oldFarClip;
                cleanProxyCamera.clearFlags = oldClear;
            }

            cleanProxyCamera.nearClipPlane = oldNearClip;
        }
        void BlitBackground(GameFrame frame)
        {
            bool oldSRGB = GL.sRGBWrite;
            GL.sRGBWrite = true;
            Graphics.SetRenderTarget(Output as RenderTexture);
            RenderTexture src = frame.backgroundBuffer;
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), src, blitBackgroundMat);
            GL.sRGBWrite = oldSRGB;
        }

        void RenderForeground(GameFrame targetFrame)
        {
            float oldFarClip = cleanProxyCamera.farClipPlane;
            LayerMask oldCull = cleanProxyCamera.cullingMask;
            CameraClearFlags oldClear = cleanProxyCamera.clearFlags;
            Color oldBGCol = cleanProxyCamera.backgroundColor;

            Color blackAndClear = new Color(0, 0, 0, 0);

            Graphics.SetRenderTarget(targetFrame.foregroundBuffer);
            GL.Clear(true, true, blackAndClear);

            cleanProxyCamera.farClipPlane = Mathf.Max(cleanProxyCamera.nearClipPlane + 0.001f, targetFrame.playerDist + 0.5f * layerOverlap);
            cleanProxyCamera.clearFlags = CameraClearFlags.Color | CameraClearFlags.Depth;   //Just cleared
            cleanProxyCamera.backgroundColor = blackAndClear;

            if ( forceToBackground > 0 )
            {
                cleanProxyCamera.cullingMask = forceToBackground;
                //TODO: optimize by disabling all lighting in this render
                cleanProxyCamera.targetTexture = targetFrame.foregroundBuffer;
                cleanProxyCamera.RenderWithShader(blockoutBackgroundShader, null);

                //now depth buffer wont allow writes where the ground would have been
                cleanProxyCamera.clearFlags = CameraClearFlags.Nothing;
            }

            Shader originalScreenSpaceShadowShader = GraphicsSettings.GetCustomShader(BuiltinShaderType.ScreenSpaceShadows);
            GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, noForegroundShadowFadeShader);

            cleanProxyCamera.cullingMask = oldCull ^ forceToBackground;

            if( takeUnfilteredAlpha )
                cleanProxyCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, grabAlphaCommand);  //Instruction to copy out the state of the RenderTexture before Image Effects are applied

            RenderGameCamera(cleanProxyCamera, targetFrame.foregroundBuffer);

            if (takeUnfilteredAlpha)
            {
                cleanProxyCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, grabAlphaCommand);
                Graphics.Blit(lastFrameAlpha, targetFrame.foregroundBuffer, copyAlphaMat);      //Overwrite the potentially broken post-effects alpha channel with the pre-effect copy
            }

            GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, originalScreenSpaceShadowShader);

            cleanProxyCamera.farClipPlane = oldFarClip;
            cleanProxyCamera.cullingMask = oldCull;
            cleanProxyCamera.clearFlags = oldClear;
            cleanProxyCamera.backgroundColor = oldBGCol;
        }
        void BlitForeground(GameFrame frame)
        {
            bool oldSRGB = GL.sRGBWrite;
            GL.sRGBWrite = true;
            Graphics.SetRenderTarget(Output as RenderTexture);
            RenderTexture src = frame.foregroundBuffer;
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), src, blitForegroundMat);
            GL.sRGBWrite = oldSRGB;
        }

        void BlitFeed(InputFeed feed)
        {
            if (!feed.enabled)
            {
                return;
            }

            feed.StartRender();
            Graphics.SetRenderTarget(Output as RenderTexture);

            if (feed.Texture != null)
            {
                bool oldSRGB = GL.sRGBWrite;
                GL.sRGBWrite = true;
                Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), feed.Texture, feed.blitMaterial);
                GL.sRGBWrite = oldSRGB;
            }

            feed.StopRender();
        }

        void RenderFinalPass()
        {
            int oldCulling = gameCamera.cullingMask;
            CameraClearFlags oldClear = gameCamera.clearFlags;
            float oldFarClip = gameCamera.farClipPlane;
            float oldNearClip = gameCamera.nearClipPlane;
            RenderingPath oldRenderPath = gameCamera.renderingPath;

            gameCamera.cullingMask = 0;
            gameCamera.clearFlags = CameraClearFlags.Nothing;
            gameCamera.farClipPlane = 1;
            gameCamera.nearClipPlane = 0.1f;
            gameCamera.renderingPath = RenderingPath.DeferredShading;

            gameCamera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, finalizeFrameCommand);

            gameCamera.targetTexture = Output as RenderTexture;
            gameCamera.aspect = (float)Output.width / Output.height;
            gameCamera.Render();

            gameCamera.targetTexture = null;

            gameCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, finalizeFrameCommand);

            gameCamera.cullingMask = oldCulling;
            gameCamera.clearFlags = oldClear;
            gameCamera.farClipPlane = oldFarClip;
            gameCamera.nearClipPlane = oldNearClip;
            gameCamera.renderingPath = oldRenderPath;
        }

        float CalculatePlayerDistance()
        {
            if (ActiveFeeds.Count == 0)
                return 0;

            float averageDist = 0;
            foreach (InputFeed feed in ActiveFeeds)
                averageDist += feed.CalculatePlayerDistance();
            averageDist /= ActiveFeeds.Count;

            return averageDist;
        }
    }
}