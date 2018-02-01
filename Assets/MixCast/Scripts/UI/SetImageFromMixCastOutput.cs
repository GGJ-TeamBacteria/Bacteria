/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    [RequireComponent(typeof(RawImage))]
    public class SetImageFromMixCastOutput : CameraComponent
    {
        public bool setScale = false;

        private MixCastCamera cam;
        private RawImage image;

        protected override void OnEnable()
        {
            image = GetComponent<RawImage>();

            base.OnEnable();

            HandleDataChanged();
        }


        private void LateUpdate()
        {
            if (cam == null || cam.context.Data != context.Data || !cam.isActiveAndEnabled)
                cam = MixCastCamera.ActiveCameras.Find(c => c.context.Data == context.Data);

            if (cam != null)
            {
                image.texture = cam.Output;
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