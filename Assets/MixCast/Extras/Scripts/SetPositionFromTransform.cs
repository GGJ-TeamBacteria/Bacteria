/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;
using System.Collections;

namespace BlueprintReality.MixCast
{
    [ExecuteInEditMode]
    public class SetPositionFromTransform : MonoBehaviour
    {
        public Transform source;
        public bool applyLocally = false;

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

            Vector3 relativePos = multiplier;
            if( source is RectTransform )
            {
                RectTransform sourceTransform = source as RectTransform;
                relativePos.x *= sourceTransform.rect.width;
                relativePos.y *= sourceTransform.rect.height;
            }

            Vector3 result = source.TransformPoint(relativePos) + source.rotation * offset;
            if (applyLocally)
                transform.localPosition = source.InverseTransformDirection(result - source.position);
            else
                transform.position = result;
        }
    }
}