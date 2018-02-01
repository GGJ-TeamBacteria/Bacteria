/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    [ExecuteInEditMode]
    public class SetPositionFromGroundHit : MonoBehaviour
    {
        public Transform startTransform;
        public Vector3 direction = Vector3.down;
        public LayerMask layers;
        public float backupDistance = 0;
        public float maxDistance = 10;

        public GameObject activeOnHit;

        private void OnEnable()
        {
            if (startTransform == null)
                startTransform = transform.parent;

            LateUpdate();
        }

        private void LateUpdate()
        {
            RaycastHit hitInfo;
            if (UnityEngine.Physics.Raycast(startTransform.position - backupDistance * direction, direction, out hitInfo, maxDistance, layers, QueryTriggerInteraction.UseGlobal))
            {
                transform.position = hitInfo.point;
                transform.up = hitInfo.normal;

                if( activeOnHit != null )
                    activeOnHit.SetActive(true);
            }
            else
                if (activeOnHit != null)
                    activeOnHit.SetActive(false);
        }
    }
}