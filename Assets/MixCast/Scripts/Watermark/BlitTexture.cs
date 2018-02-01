using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class BlitTexture
    {
        public Texture2D Texture { get; set; }

        public Vector2 size = new Vector2(0.1f, 0.1f);
        public float xPixelOffset = 15f;
        public float yPixelOffset = 15f;

        public enum Position { BottomLeft, BottomRight };
        public Position texturePosition = Position.BottomLeft;

        public void SetTexturePosition (Position newPosition)
        {
            texturePosition = newPosition;
        }

        public BlitTexture()
        {
        }

        public void Enable()
        {
            MixCastCamera.FrameCompleted += OnGameRenderEnd;
        }

        public void Disable()
        {
            MixCastCamera.FrameCompleted -= OnGameRenderEnd;
        }

        /// <summary>
        /// Info below is important, but not longer relevant here after greatly simplifying the original implementation of this class -
        /// Immediate camera and Buffered camera have different origin for RenderTexture.
        /// Immediate origin is in the center of its RenderTexture, Buffered is top left corner (the same as Quandrant).
        /// </summary>
        /// 
        /// <returns>Rect for DrawTexture.</returns>
        private Rect CalculateRect(MixCastCamera camera, Vector2 targetSize, Vector2 watermarkSize)
        {
            switch (texturePosition)
            {
                case Position.BottomLeft:
                    return new Rect(xPixelOffset, targetSize.y - watermarkSize.y - yPixelOffset, watermarkSize.x, watermarkSize.y);
                case Position.BottomRight:
                    return new Rect(targetSize.x - watermarkSize.x - xPixelOffset, targetSize.y - watermarkSize.y - yPixelOffset, watermarkSize.x, watermarkSize.y);
                default:
                    return new Rect(xPixelOffset, targetSize.y - watermarkSize.y - yPixelOffset, watermarkSize.x, watermarkSize.y);
            }
            
        }

        private void OnGameRenderEnd(MixCastCamera camera)
        {
            if (Texture == null)
            {
                return;
            }

            var renderTex = camera.Output as RenderTexture;

            Rect rect = CalculateRect(
                            camera,
                            new Vector2(renderTex.width, renderTex.height),
                            new Vector2(Texture.width, Texture.height));

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, renderTex.width, renderTex.height, 0);
            Graphics.SetRenderTarget(renderTex);
            bool oldSRGB = GL.sRGBWrite;
            GL.sRGBWrite = true;
            Graphics.DrawTexture(rect, Texture);
            GL.sRGBWrite = oldSRGB;
            GL.End();
            GL.PopMatrix();
        }
    }
}