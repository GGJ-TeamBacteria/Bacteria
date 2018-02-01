/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace BlueprintReality.MixCast
{
    [ExecuteInEditMode]
    public class ImmediateMixCastCamera : MixCastCamera
    {
        //Simple MixedRealityCamera that renders the game camera into the Output. Additional logic can be attached to the game camera as CommandBuffers in order to insert the real feed
        private Material postBlit;
        private CommandBuffer postBuff;

        private RenderingPath lastRenderPath;
        private CommandBuffer feedCommand;

        protected override void Awake()
        {
            base.Awake();
        
            postBlit = new Material(Shader.Find("Hidden/BPR/AlphaWrite"));
            postBuff = new CommandBuffer();
            postBuff.Blit(null, BuiltinRenderTextureType.CameraTarget, postBlit);

            feedCommand = new CommandBuffer();
        }
        private void OnDestroy()
        {
            feedCommand.Dispose();

            postBuff.Dispose();
            postBuff = null;
            postBlit = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddBlitCommand();

            gameCamera.AddCommandBuffer(CameraEvent.AfterEverything, postBuff);
        }
        protected override void OnDisable()
        {
            gameCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, postBuff);

            RemoveBlitCommand();

            base.OnDisable();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (lastRenderPath != gameCamera.actualRenderingPath)
            {
                RemoveBlitCommand();    //Remove bad command
                AddBlitCommand();       //Add good command
            }
        }

        public override void RenderScene()
        {
            //Update shader properties for Feeds to access
            Shader.SetGlobalFloat("_CamNear", gameCamera.nearClipPlane);
            Shader.SetGlobalFloat("_CamFar", gameCamera.farClipPlane);
            Shader.SetGlobalMatrix("_CamToWorld", gameCamera.cameraToWorldMatrix);
            Shader.SetGlobalMatrix("_WorldToCam", gameCamera.worldToCameraMatrix);
            Shader.SetGlobalMatrix("_CamProjection", gameCamera.projectionMatrix);

            for (int i = 0; i < ActiveFeeds.Count; i++)
                ActiveFeeds[i].StartRender();
            RenderGameCamera(gameCamera, Output as RenderTexture);
            for (int i = 0; i < ActiveFeeds.Count; i++)
                ActiveFeeds[i].StopRender();
            Graphics.SetRenderTarget(null);
            CompleteFrame();
        }

        void AddBlitCommand()
        {
            if (gameCamera.actualRenderingPath == RenderingPath.Forward)
                gameCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, feedCommand);
            else
                gameCamera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, feedCommand);

            lastRenderPath = gameCamera.actualRenderingPath;
        }
        void RemoveBlitCommand()
        {
            if (lastRenderPath == RenderingPath.Forward)
                gameCamera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, feedCommand);
            else
                gameCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, feedCommand);
        }

        public override void RegisterFeed(InputFeed feed)
        {
            base.RegisterFeed(feed);
            RebuildFeedCommand();
        }
        public override void UnregisterFeed(InputFeed feed)
        {
            base.UnregisterFeed(feed);
            RebuildFeedCommand();
        }
        void RebuildFeedCommand()
        {
            feedCommand.Clear();
            for (int i = 0; i < ActiveFeeds.Count; i++)
                feedCommand.Blit(ActiveFeeds[i].Texture, BuiltinRenderTextureType.CurrentActive, ActiveFeeds[i].blitMaterial);
        }
    }
}