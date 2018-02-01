using System.Collections;
using UnityEngine;

namespace BlueprintReality.Performance
{
    public class StatsTracker : MonoBehaviour
    {
        public virtual string Key { get; set; }

        public int Current { get; protected set; }
        public int Lowest { get; protected set; }
        public int Highest { get; protected set; }
        public int Avg { get; protected set; }
        public int Target { get; protected set; }
        public int SampleCount { get; protected set; }

        public bool IsReady { get { return SampleCount > 0; } }
        public bool IsTracking { get; protected set; }

        private int avgSum = 0;

        private Coroutine printCoroutine;

        protected void SetCurrent(int newCurrent)
        {
            ++SampleCount;

            Current = newCurrent;

            avgSum += Current;
            Avg = avgSum / SampleCount;

            if (Current < Lowest)
            {
                Lowest = Current;
            }

            if (Current > Highest)
            {
                Highest = Current;
            }
        }

        public void ResetStats()
        {
            Lowest = int.MaxValue;
            Highest = int.MinValue;
            Avg = 0;
            SampleCount = 0;
            avgSum = 0;
        }

        public virtual void StartTracking()
        {
            IsTracking = true;
        }

        public virtual void StopTracking()
        {
            IsTracking = false;
        }

        public void StartPrinting()
        {
            if (printCoroutine != null)
            {
                return;
            }

            printCoroutine = StartCoroutine(Print());
        }

        public void StopPrinting()
        {
            StopCoroutine(printCoroutine);

            printCoroutine = null;
        }

        private IEnumerator Print()
        {
            while (true)
            {
                Debug.Log(string.Format("KEY:{4} CUR:{0} AVG:{1} MIN:{2} MAX:{3}", Current, Avg, Lowest, Highest, Key));

                yield return new WaitForSeconds(1);
            }
        }
    }
}