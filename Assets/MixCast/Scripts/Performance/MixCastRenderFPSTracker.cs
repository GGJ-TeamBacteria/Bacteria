using BlueprintReality.MixCast;

namespace BlueprintReality.Performance
{
    public class MixCastRenderFPSTracker : FPSStatsTracker
    {
        public const string KeyName = "MixCast Render";

        public override string Key { get { return KeyName; } }

        public override void StartTracking()
        {
            base.StartTracking();

            if (MixCastCameras.Instance != null)
            {
                MixCastCameras.Instance.OnBeforeRender += Tick;
            }
        }

        public override void StopTracking()
        {
            base.StopTracking();

            if (MixCastCameras.Instance != null)
            {
                MixCastCameras.Instance.OnBeforeRender -= Tick;
            }
        }

        void Update()
        {
            Target = MixCast.MixCast.Settings.global.targetFramerate;
        }
        
        void OnDestroy()
        {
            if (MixCastCameras.Instance != null)
            {
                MixCastCameras.Instance.OnBeforeRender -= Tick;
            }
        }
    }
}