using BlueprintReality.MixCast;

namespace BlueprintReality.Performance
{
    public class DroppedFramesTracker : StatsTracker
    {
        public const string KeyName = "Dropped Frames";

        public override string Key { get { return KeyName; } }
        public int FrameWindow = 3;
        private int frameWindowCounter;
        private int droppedFramesThisWindow;
        
        void LateUpdate()
        {
            if (!IsTracking)
            {
                return;
            }

            int dropped = GetDroppedFrames();
            
            droppedFramesThisWindow += dropped;

            ++frameWindowCounter;

            if (frameWindowCounter >= FrameWindow)
            {
                frameWindowCounter = 0;

                SetCurrent(droppedFramesThisWindow);

                droppedFramesThisWindow = 0;
            }
        }

        private int GetDroppedFrames()
        {
#if MIXCAST_STEAMVR
            if (VRInfo.IsDeviceOpenVR())
            {
                return GetDroppedFramesSteamVR();
            }
#elif MIXCAST_OCULUS
            if (VRInfo.IsDeviceOculus())
            {
                return GetDroppedFramesOculus();
            }
#endif

            return 0;
        }

#if MIXCAST_OCULUS
        private int GetDroppedFramesOculus()
        {
            var stats = OVRPlugin.GetAppPerfStats();

            if (stats.FrameStatsCount > 0)
            {
                return stats.FrameStats[stats.FrameStatsCount - 1].AppDroppedFrameCount;
            }

            return 0;
        }
#endif

#if MIXCAST_STEAMVR
        private int GetDroppedFramesSteamVR()
        {
            var compositor = Valve.VR.OpenVR.Compositor;
            if (compositor != null)
            {
                var timing = new Valve.VR.Compositor_FrameTiming();
                timing.m_nSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.Compositor_FrameTiming));

                compositor.GetFrameTiming(ref timing, 0);

                return (int)timing.m_nNumDroppedFrames;
            }

            return 0;
        }
#endif
    }
}
