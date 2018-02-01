using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace BlueprintReality.Performance
{
    public class PerformanceNotifier : MonoBehaviour
    {
        public bool shouldSaveToFile = true;

        private const string Filename = "Performance.txt";
        private const string FilenamePrev = "PerformancePrev.txt";

        private bool hasMovedOldFile;
        private string filepath;


        [System.Serializable]
        public class NotifyCondition
        {
            public enum Comparator
            {
                Lower   = 1,
                Higher  = 2
            }

            public PerformanceTracker.Types trackerType;
            public Comparator comp = Comparator.Higher;
            public int target;
            public int repeatCount;

            [HideInInspector]
            public StatsTracker tracker;
            [HideInInspector]
            public int currentCount;
            [HideInInspector]
            public int currentSampleCount;
        }

        public NotifyCondition[] conditions;
        
        void Awake()
        {
            filepath = Path.Combine(Application.persistentDataPath, Filename);
        }

        void Start()
        {
            Init();
        }

        public void Init()
        {
            if (PerformanceTracker.Instance == null)
            {
                Debug.LogWarning("Warning: Performance Tracker does not exists!");

                return;
            }

            foreach (var cond in conditions)
            {
                cond.tracker = PerformanceTracker.Instance.GetTracker(cond.trackerType);
            }
        }
        
        void Update()
        {
            if (CheckConditions())
            {
                SendNotification();
            }
        }
        
        private bool CheckConditions()
        {
            foreach (var cond in conditions)
            {
                if (cond.tracker == null
                    || cond.tracker.SampleCount == cond.currentSampleCount)
                {
                    continue;
                }

                cond.currentSampleCount = cond.tracker.SampleCount;

                if (cond.comp == NotifyCondition.Comparator.Higher)
                {
                    if (cond.tracker.Current > cond.target)
                    {
                        ++cond.currentCount;
                    }
                    else
                    {
                        cond.currentCount = 0;
                    }
                }
                else
                {
                    if (cond.tracker.Current < cond.target)
                    {
                        ++cond.currentCount;
                    }
                    else
                    {
                        cond.currentCount = 0;
                    }
                }

                if (cond.currentCount >= cond.repeatCount)
                {
                    cond.currentCount = 0;

                    //Debug.Log("SLOW! " + cond.tracker + " target: " + cond.tracker.Current + "/" + cond.target);

                    return true;
                }
            }
            
            return false;
        }
        
        private void SendNotification()
        {
            string message = string.Format("{0}: Stats: {1}", DateTime.Now.ToString(), PrintTrackers());
            
            if (shouldSaveToFile)
            {
                WriteToFile(message);
            }
        }

        private void WriteToFile(string message)
        {
            if (!hasMovedOldFile)
            {
                hasMovedOldFile = true;

                MoveOldFile();
            }
            
            const long MaxFileSize = 50 * 1024 * 1024;
            
            try
            {
                var fileInfo = new FileInfo(filepath);

                if (fileInfo.Exists
                    && fileInfo.Length > MaxFileSize)
                {
                    MoveOldFile();
                }

                StreamWriter writer = new StreamWriter(filepath, true);
                writer.WriteLine(message);
                writer.Close();
            }
            catch (Exception e)
            {
                Debug.LogError("Error during writing performance nofitication into a file! Exception: " + e.Message);
            }
        }

        private void MoveOldFile()
        {
            string oldFilepath = Path.Combine(Application.persistentDataPath, FilenamePrev);

            try
            {
                if (File.Exists(oldFilepath))
                {
                    File.Delete(oldFilepath);
                }

                if (File.Exists(filepath))
                {
                    File.Move(filepath, oldFilepath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error during moving old performance notification file! Expection: " + e.Message);
            }
        }

        private string PrintTrackers()
        {
            var sb = new StringBuilder();

            foreach (var tracker in PerformanceTracker.Instance.Trackers)
            {
                sb.AppendFormat("{0}: {1} (Min {2}, Max {3}, Avg {4}) ", tracker.Key, tracker.Current, tracker.Lowest, tracker.Highest, tracker.Avg);
            }

            return sb.ToString();
        }
    }
}