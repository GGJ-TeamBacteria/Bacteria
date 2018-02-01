using UnityEngine;
using UnityEngine.VR;

namespace BlueprintReality.Performance
{
    public class GameFPSTracker : FPSStatsTracker
    {
        public const string KeyName = "Game";

        public override string Key { get { return KeyName; }}

        void Start()
        {
            Target = Mathf.RoundToInt(UnityEngine.XR.XRDevice.refreshRate);
        }
        
        void Update()
        {
            Tick();
        }
    }
}