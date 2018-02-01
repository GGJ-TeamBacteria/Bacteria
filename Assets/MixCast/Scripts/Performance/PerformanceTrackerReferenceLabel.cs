using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.Performance
{
    /// <summary>
    /// Label for Peformance Stats when direct reference to StatsTracker component can be make in Editor inspector.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class PerformanceTrackerReferenceLabel : MonoBehaviour
    {
        public StatsTracker[] trackers;
        public string format = "{0}: {1} ({2}/{3}/{4})";
        public float updateInterval = 0.1f;

        private float nextUpdateTime;
        private UnityEngine.UI.Text label;
        private StringBuilder sb = new StringBuilder();

        void Awake()
        {
            label = GetComponent<UnityEngine.UI.Text>();
            nextUpdateTime = Time.unscaledTime + nextUpdateTime;
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
            sb.Length = 0;

            foreach (var tracker in trackers)
            {
                if (tracker.IsReady)
                {
                    sb.AppendFormat(format, tracker.Key, tracker.Current, tracker.Lowest, tracker.Highest, tracker.Avg, tracker.Target);
                }
                else
                {
                    sb.AppendFormat(format, tracker.Key, "-", "-", "-", "-", tracker.Target);
                }

                sb.AppendLine();
            }

            label.text = sb.ToString();
        }
    }
}