using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class OutputMixCastToScreenshot : CameraComponent
    {
        private static List<OutputMixCastToScreenshot> encodeQueue = new List<OutputMixCastToScreenshot>();

        public string filename = "image.jpg";

        private List<IAsyncResult> activeSaves = new List<IAsyncResult>();

        protected override void OnDisable()
        {
            activeSaves.ForEach(ar =>
            {
                FileStream fs = ar.AsyncState as FileStream;
                fs.Flush();
                fs.Close();
                fs.Dispose();
            });
            activeSaves.Clear();
            base.OnDisable();
        }

        public virtual void Run()
        {
            StartCoroutine(RunAsync());
        }
        IEnumerator RunAsync()
        {
            MixCastCamera cam = MixCastCamera.ActiveCameras.Find(c => c.context.Data == context.Data);
            if (cam == null)
                yield break;

            RenderTexture srcTex = RenderTexture.GetTemporary(cam.Output.width, cam.Output.height, 0);
            Graphics.Blit(cam.Output, srcTex);

            //Distribute encoding so only one texture encodes per frame (since not threadable)
            encodeQueue.Add(this);
            yield return new WaitForEndOfFrame();
            while (encodeQueue[0] != this)
            {
                if (encodeQueue[0] == null)
                    encodeQueue.RemoveAt(0);    //mechanism so 2nd instance still doesn't trigger same frame
                yield return null;
            }

            //Reserve file
            string finalFilename = MixCastFiles.GetAvailableFilename(filename);
            FileStream fs = new FileStream(finalFilename,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None, cam.Output.width * cam.Output.height * 4,
                true);

            yield return null;

            Texture2D tex = new Texture2D(cam.Output.width, cam.Output.height, TextureFormat.RGB24, false, QualitySettings.activeColorSpace == ColorSpace.Linear);

            yield return null;

            RenderTexture.active = srcTex;
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            RenderTexture.active = null;

            yield return null;

            srcTex.Release();
            srcTex = null;

            yield return null;

            byte[] texData = tex.EncodeToJPG(100);
            DestroyImmediate(tex);

            activeSaves.Add(fs.BeginWrite(texData, 0, texData.Length, ScreenshotSaved, fs));

            encodeQueue[0] = null;  //Release encoding lock
        }
        void Encode()
        {

        }
        void ScreenshotSaved(IAsyncResult ar)
        {
            FileStream fs = ar.AsyncState as FileStream;
            fs.EndWrite(ar);
            fs.Close();
            fs.Dispose();
            activeSaves.Remove(ar);
        }
    }
}