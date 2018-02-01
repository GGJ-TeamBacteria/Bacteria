/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class CameraConfigContext : MonoBehaviour
    {
        private MixCastData.CameraCalibrationData data;

        public MixCastData.CameraCalibrationData Data
        {
            get
            {
                return data;
            }
            set
            {
                if (data == value)
                    return;

                data = value;
                
                if (DataChanged != null)
                    DataChanged();
            }
        }
        public event System.Action DataChanged;
    }
}