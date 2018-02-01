using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BlueprintReality.MixCast
{
    public class BlitControls : MonoBehaviour
    {

        private bool showLogo = true;

        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void Update() {

        }

        public void ShowBrandingBlit()
        {
            MixCastCameras.GetBrandingBlit().Enable();
        }

        public void HideBrandingBlit()
        {
            MixCastCameras.GetBrandingBlit().Disable();
        }

        public void ShowLogoBlit()
        {
            if (!MixCast.SecureSettings.IsFreeLicense)
            {
                MixCastCameras.GetLogoBlit().Enable();
                showLogo = true;
            }
                
        }

        public void HideLogoBlit()
        {
            if (!MixCast.SecureSettings.IsFreeLicense)
            {
                MixCastCameras.GetLogoBlit().Disable();
                showLogo = false;
            }
                
        }

        public void ToggleLogoBlit()
        {
            if (showLogo)
            {
                HideLogoBlit();
            }
            else
            {
                ShowLogoBlit();
            }
        }

        public void ToggleLogoIfMixCastActive()
        {
            
            if (MixCast.Active)
            {
                
                ToggleLogoBlit();
            }
        }
    }
}
