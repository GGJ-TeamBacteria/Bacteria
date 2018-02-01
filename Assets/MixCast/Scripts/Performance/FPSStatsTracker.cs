using UnityEngine;

namespace BlueprintReality.Performance
{
    public class FPSStatsTracker : StatsTracker
    {
        private const float fpsMeasurePeriod = 0.5f;
        private int fpsAccumulator = 0;
        private float fpsNextMeasureTime = 0;
        
        public void Tick()
        {
            ++fpsAccumulator;
        }

        public override void StartTracking()
        {
            base.StartTracking();

            fpsAccumulator = 0;
            fpsNextMeasureTime = Time.unscaledTime + fpsMeasurePeriod;
        }

        void LateUpdate()
        {
            if (!IsTracking)
            {
                return;
            }

            if (Time.unscaledTime >= fpsNextMeasureTime)
            {
                MeasureFps();
            }
        }

        private void MeasureFps()
        {
            int currentFPS = Mathf.RoundToInt(fpsAccumulator / fpsMeasurePeriod);

            SetCurrent(currentFPS);
            
            fpsAccumulator = 0;
            fpsNextMeasureTime += fpsMeasurePeriod;
        }
    }
}