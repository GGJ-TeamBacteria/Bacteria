using System.Collections;
using UnityEngine;
using System.Diagnostics;

namespace BlueprintReality.Performance
{
    /// <summary>
    /// Test class to check if FPSStatsTracker calculates FPS data correctly.
    /// </summary>
    [RequireComponent(typeof(FPSStatsTracker))]
    public class FPSTrackerTester : MonoBehaviour
    {
        public int targetFPS = 60;
        public int measuredFPS = 0;
        public int avgFPS = 0;
        public int internalFPS = 0;

        private int internalFPSCounter;
        private float nextTickTime;
        private FPSStatsTracker tracker;
        private Stopwatch stopwatch;

        void Awake()
        {
            tracker = GetComponent<FPSStatsTracker>();

            stopwatch = new Stopwatch();
        }

        void Start()
        {
            stopwatch.Start();
            
            nextTickTime = Time.unscaledTime + 1f / targetFPS;

            tracker.StartTracking();
        }
        
        private void Tick()
        {
            ++internalFPSCounter;

            tracker.Tick();
        }

        void Update()
        {
            measuredFPS = tracker.Current;
            avgFPS = tracker.Avg;
        }

        void LateUpdate()
        {
            if (stopwatch.ElapsedMilliseconds >= 1000)
            {
                internalFPS = internalFPSCounter;
                internalFPSCounter = 0;

                stopwatch.Reset();
                stopwatch.Start();
            }

            if (Time.unscaledTime >= nextTickTime)
            {
                Tick();

                nextTickTime += 1f / targetFPS;
            }
        }
    }
}