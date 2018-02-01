/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;

namespace BlueprintReality.MixCast
{
    [ExecuteInEditMode]
    public class SetGroundOriginFromRaycast : MonoBehaviour
    {
        public InputFeed feed;

        public string groundPositionParameter = "_GroundScreenHeight"; //0 is bottom of screen, 1 is top

        public LayerMask groundLayers = 0;
        public float maxRayLength = 10;

        private void Update()
        {
            if (feed == null || !feed.isActiveAndEnabled)
                return;

            Vector3 groundOrigin = Vector3.zero;
            if (groundLayers == 0)
            {
                groundOrigin = Camera.main.transform.position;
                groundOrigin.y = 0;
            }
            else
            {
                RaycastHit hitInfo;
                if (UnityEngine.Physics.Raycast(Camera.main.transform.position, Vector3.down, out hitInfo, maxRayLength, groundLayers, QueryTriggerInteraction.Ignore))
                    groundOrigin = hitInfo.point;
                else
                    groundOrigin = Camera.main.transform.position + Vector3.down * maxRayLength;
            }

            if( feed.blitMaterial != null && feed.blitMaterial.HasProperty(groundPositionParameter) )
            {
                float output = 0;
                if (feed.cam.gameCamera.transform.InverseTransformPoint(groundOrigin).z > 0)
                    output = Mathf.Clamp01(feed.cam.gameCamera.WorldToViewportPoint(groundOrigin).y);
                feed.blitMaterial.SetFloat(groundPositionParameter, output);
            }
        }
    }
}