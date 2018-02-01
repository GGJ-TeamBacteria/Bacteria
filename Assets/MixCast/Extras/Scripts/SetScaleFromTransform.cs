/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;
using System.Collections;

namespace BlueprintReality.MixCast
{
    [ExecuteInEditMode]
    public class SetScaleFromTransform : MonoBehaviour
    {
        public Transform source;
        public bool applyLocally = true;

        public Vector3 offset = Vector3.zero;
        public Vector3 multiplier = Vector3.one;

        void OnEnable()
        {
            Update();
        }

        void Update()
        {
            if (source == null)
                return;

            Vector3 relative = multiplier;
            if( source is RectTransform )
            {
                RectTransform sourceTransform = source as RectTransform;
                relative.x *= sourceTransform.rect.width;
                relative.y *= sourceTransform.rect.height;
            }

            if (applyLocally)
                transform.localScale = offset + Vector3.Scale(relative, source.localScale);
            else
            {
                transform.localScale = offset + Vector3.Scale(relative, source.lossyScale);
            }
        }
    }
}