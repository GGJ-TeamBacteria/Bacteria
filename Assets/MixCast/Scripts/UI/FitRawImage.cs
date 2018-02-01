using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    [RequireComponent(typeof(RawImage))]
    public class FitRawImage : MonoBehaviour
    {
        public enum Mode
        {
            FitToParent = 0,
        }

        public Mode mode;

        public void Fit()
        {
            var image = GetComponent<RawImage>();

            if (image.texture == null)
            {
                return;
            }

            switch (mode)
            {
                case Mode.FitToParent:

                    FitToParent(image);

                    break;
            }
        }

        public void FitToParent(RawImage image)
        {
            float texAspect = 1f * image.texture.width / image.texture.height;

            var parentRect = transform.parent.GetComponent<RectTransform>();
            if (parentRect == null)
            {
                return;
            }

            var imageRect = image.rectTransform;

            float parentAspect = parentRect.rect.width / parentRect.rect.height;

            Vector2 size;
            Vector3 pos = imageRect.position;

            if (texAspect >= parentAspect)
            {
                size.x = parentRect.rect.width;
                size.y = parentRect.rect.width / texAspect;

                pos.y += (parentRect.rect.height - size.y) / 2f;
            }
            else
            {
                size.x = parentRect.rect.height * texAspect;
                size.y = parentRect.rect.height;

                pos.x += (parentRect.rect.width - size.x) / 2f;
            }

            imageRect.position = pos;
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
    }
}