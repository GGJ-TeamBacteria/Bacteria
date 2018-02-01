using UnityEngine;

namespace BlueprintReality.Performance
{
    /// <summary>
    /// Label for Peformance Stats as indirect reference to StatsTracker component.
    /// This object will grab reference to StatsTracker at runtime.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class PerformanceTrackerLabel : MonoBehaviour
    {
        public PerformanceTracker.Types type;

        [Tooltip("{0} - Name, {1} - Current, {2} - Min, {3} - Max, {4} - Avg, {5} - Target")]
        public string format = "{0}: {1}({5}) [{2}/{3}/{4}]";
        public float updateInterval = 0.1f;

        private StatsTracker tracker;
        private float nextUpdateTime;
        private UnityEngine.UI.Text label;

        void Awake()
        {
            label = GetComponent<UnityEngine.UI.Text>();
            nextUpdateTime = Time.unscaledTime + nextUpdateTime;
        }

        void Start()
        {
            if (tracker == null)
            {
                GetTracker();
            }
        }

        void OnEnable()
        {
            if (tracker == null)
            {
                GetTracker();
            }
        }

        void OnDestroy()
        {
            if (PerformanceTracker.Instance != null)
            {
                PerformanceTracker.Instance.OnTrackerRegistered -= TrackerRegistered;
            }
        }

        private void GetTracker()
        {
            if (PerformanceTracker.Instance == null)
            {
                return;
            }

            tracker = PerformanceTracker.Instance.GetTracker(type);

            if (tracker == null)
            {
                PerformanceTracker.Instance.OnTrackerRegistered -= TrackerRegistered;
                PerformanceTracker.Instance.OnTrackerRegistered += TrackerRegistered;
            }
        }

        private void TrackerRegistered(StatsTracker registeredTracker)
        {
            PerformanceTracker.Instance.OnTrackerRegistered -= TrackerRegistered;

            if (registeredTracker.Key == PerformanceTracker.GetKeyNameFromEnum(type))
            {
                tracker = registeredTracker;
            }
        }

        void Update()
        {
            if (Time.unscaledTime >= nextUpdateTime)
            {
                nextUpdateTime += updateInterval;

                UpdateLabel();
            }
        }

        private void UpdateLabel()
        {
            if (tracker == null)
            {
                label.text = "";

                return;
            }

            if (tracker.IsReady)
            {
                label.text = string.Format(format, tracker.Key, tracker.Current, tracker.Lowest, tracker.Highest, tracker.Avg, tracker.Target);
            }
            else
            {
                label.text = string.Format(format, tracker.Key, "-", "-", "-", "-", tracker.Target);
            }
        }
    }
}