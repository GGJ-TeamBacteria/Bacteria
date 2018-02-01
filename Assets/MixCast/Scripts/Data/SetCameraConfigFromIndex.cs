/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;

namespace BlueprintReality.MixCast
{
    [RequireComponent(typeof(CameraConfigContext))]
    public class SetCameraConfigFromIndex : MonoBehaviour
    {
        public int index;

        private CameraConfigContext context;

        private void Awake()
        {
            context = GetComponent<CameraConfigContext>();
        }

        private void Update()
        {
            if (index < MixCast.Settings.cameras.Count)
                context.Data = MixCast.Settings.cameras[index];
            else
                context.Data = null;
        }
    }
}