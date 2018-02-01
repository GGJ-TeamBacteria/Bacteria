/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BlueprintReality.MixCast
{
    public class WebcamInputFeed : InputFeed
    {
        public WebCamTexture WebcamTexture
        {
            get; protected set;
        }

        public override Texture Texture
        {
            get
            {
                return WebcamTexture;
            }
        }
        
        protected override void SetTexture()
        {
            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                if (WebCamTexture.devices[i].name == context.Data.deviceName)
                {
                    if (context.Data.deviceFeedWidth == 0 || context.Data.deviceFeedHeight == 0)
                        WebcamTexture = new WebCamTexture(context.Data.deviceName);
                    else
                        WebcamTexture = new WebCamTexture(context.Data.deviceName, context.Data.deviceFeedWidth, context.Data.deviceFeedHeight, 30);
                    WebcamTexture.wrapMode = TextureWrapMode.Clamp;
                    WebcamTexture.Play();

                    base.SetTexture();
                }
            }
        }

        protected override void ClearTexture()
        {
            if (Texture != null)
            {
                WebcamTexture.Stop();
                WebcamTexture = null;
            }
            base.ClearTexture();
        }
    }
}