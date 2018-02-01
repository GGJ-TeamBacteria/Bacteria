using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.Performance
{
    public class PerformanceTracker : MonoBehaviour
    {
        public enum Types
        {
            GameFPS         = 0,
            MixCastFPS      = 1,
            CPULoad         = 10,
            GPUMs           = 11,
            DroppedFrames   = 20,
        }

        public static PerformanceTracker Instance { get; private set; }

        public bool IsTracking { get; private set; }
        public List<StatsTracker> Trackers { get; private set; }

        /*
         * KeyCode bindings below still need to be set manually to the same as in KeyEvents on Performance Screen object.
         * Changing these (or those on Performance Screen) will break tracker functionality with current implementation.
         * */

        private KeyCode startStopKey = KeyCode.T;
        private KeyCode startStopKey2 = KeyCode.LeftControl;
        private KeyCode startStopKey3 = KeyCode.LeftAlt;

        private bool addTrackersOnAwake = true;

        [Tooltip("Automatically start tracking 4 seconds after Start")]

        // Changing this to true will break tracker functionality
        private bool startTrackingOnStart = false;

        public Action<StatsTracker> OnTrackerRegistered;

        private Dictionary<string, StatsTracker> trackersMap;

        void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(this);

                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            trackersMap = new Dictionary<string, StatsTracker>();
            Trackers = new List<StatsTracker>();

            var existingTrackers = GetComponentsInChildren<StatsTracker>();
            foreach (var tracker in  existingTrackers)
            {
                AddTracker(tracker.Key, tracker);
            }

            if (addTrackersOnAwake)
            {
                RegisterTrackers();
            }
        }

        void OnEnable()
        {
            if (startTrackingOnStart && !IsTracking)
            {
                StartCoroutine(StartTrackingAsync());
            }
        }

        private IEnumerator StartTrackingAsync()
        {
            yield return new WaitForSeconds(4f);

            StartTracking();
        }

        private void RegisterTrackers()
        {
            AddTracker(GameFPSTracker.KeyName, AddTrackerComponent<GameFPSTracker>());
            AddTracker(MixCastRenderFPSTracker.KeyName, AddTrackerComponent<MixCastRenderFPSTracker>());
            AddTracker(CPUTracker.KeyName, AddTrackerComponent<CPUTracker>());
            AddTracker(GPUTracker.KeyName, AddTrackerComponent<GPUTracker>());
            AddTracker(DroppedFramesTracker.KeyName, AddTrackerComponent<DroppedFramesTracker>());
        }

        private T AddTrackerComponent<T>() where T : StatsTracker
        {
            T tracker = GetComponent<T>();
            if (tracker == null)
            {
                tracker = gameObject.AddComponent<T>();
            }

            return tracker;
        }

        public void AddTracker(string key, StatsTracker tracker)
        {
            trackersMap.Add(key, tracker);
            Trackers.Add(tracker);

            if (OnTrackerRegistered != null)
            {
                OnTrackerRegistered(tracker);
            }
        }

        public StatsTracker GetTracker(string key)
        {
            StatsTracker tracker;
            trackersMap.TryGetValue(key, out tracker);

            return tracker;
        }

        public StatsTracker GetTracker(Types type)
        {
            string key = GetKeyNameFromEnum(type);

            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return GetTracker(key);
        }

        public static string GetKeyNameFromEnum(Types type)
        {
            switch (type)
            {
                case Types.GameFPS:
                    return GameFPSTracker.KeyName;

                case Types.MixCastFPS:
                    return MixCastRenderFPSTracker.KeyName;

                case Types.CPULoad:
                    return CPUTracker.KeyName;

                case Types.GPUMs:
                    return GPUTracker.KeyName;
                    
                case Types.DroppedFrames:
                    return DroppedFramesTracker.KeyName;
            }

            return null;
        }

        void Update()
        {
            if (Input.GetKeyDown(startStopKey)
                && (startStopKey2 == KeyCode.None || Input.GetKey(startStopKey2))
                && (startStopKey3 == KeyCode.None || Input.GetKey(startStopKey3)))
            {
                if (IsTracking)
                {
                    StopTracking();
                }
                else
                {
                    StartTracking();
                }
            }
        }

        public void StartTracking()
        {
            IsTracking = true;

            foreach (var tracker in Trackers)
            {
                tracker.ResetStats();
                tracker.StartTracking();
            }
        }

        public void StopTracking()
        {
            IsTracking = false;

            foreach (var tracker in Trackers)
            {
                tracker.StopTracking();
            }
        }
    }
}