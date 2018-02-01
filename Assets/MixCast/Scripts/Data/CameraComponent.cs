/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class CameraComponent : MonoBehaviour
    {
        public CameraConfigContext context;

        protected virtual void OnEnable()
        {
            if (context == null)
                context = GetComponentInParent<CameraConfigContext>();

            context.DataChanged += HandleDataChanged;
        }
        protected virtual void OnDisable()
        {
            context.DataChanged -= HandleDataChanged;
        }

        protected virtual void HandleDataChanged()
        {
            
        }


        protected virtual void Reset()
        {
            context = GetComponentInParent<CameraConfigContext>();
        }
    }
}