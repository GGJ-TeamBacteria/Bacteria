using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class OutputMixCastToTimelapse : OutputMixCastToScreenshotAuto
    {
        private const float ENABLE_COOLDOWN = 1f;

        private float captureCooldown;

        protected override void OnEnable()
        {
            base.OnEnable();

            captureCooldown = ENABLE_COOLDOWN;
        }

        private void Update()
        {
            if (context.Data == null || !context.Data.recordingData.autoStartTimelapse)
                return;

            if( captureCooldown > 0 )
            {
                if (captureCooldown > context.Data.recordingData.timelapseInterval && captureCooldown > ENABLE_COOLDOWN)
                    captureCooldown = context.Data.recordingData.timelapseInterval;

                captureCooldown -= Time.unscaledDeltaTime;
                if( captureCooldown <= 0 )
                {
                    Run();
                    captureCooldown = Mathf.Max(context.Data.recordingData.timelapseInterval, ENABLE_COOLDOWN);
                }
            }
        }

        protected override void HandleDataChanged()
        {
            if (context.Data != null || !context.Data.recordingData.autoStartTimelapse)
                captureCooldown = context.Data.recordingData.timelapseInterval;

            base.HandleDataChanged();
        }
    }
}