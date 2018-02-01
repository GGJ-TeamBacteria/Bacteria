/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    [RequireComponent(typeof(RawImage))]
    public class SetImageFromMixCastInput : CameraComponent
    {
        public bool setScale = false;

        private MixCastCamera cam;
        private InputFeed feed;
        private RawImage image;

        protected override void OnEnable()
        {
            image = GetComponent<RawImage>();

            base.OnEnable();

            HandleDataChanged();
        }


        private void LateUpdate()
        {
            if (cam == null || cam.context.Data != context.Data || !cam.isActiveAndEnabled || feed == null || !feed.isActiveAndEnabled || feed.Texture == null)
            {
                cam = MixCastCamera.ActiveCameras.Find(c => c.context.Data == context.Data);
                if (cam != null)
                    feed = cam.GetComponentInChildren<InputFeed>();
                else
                    feed = null;
            }

            if (feed != null)
            {
                image.texture = feed.Texture;
                image.SetMaterialDirty();

                if (setScale && image.texture != null)
                {
                    image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)image.texture.width / image.texture.height * image.rectTransform.rect.height);
                }
            }
            else
                image.texture = null;
        }
    }
}