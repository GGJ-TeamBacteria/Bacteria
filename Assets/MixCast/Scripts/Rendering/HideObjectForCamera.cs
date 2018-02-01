/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BlueprintReality.MixCast
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class HideObjectForCamera : MonoBehaviour
    {
        public List<Renderer> renderers = new List<Renderer>();

        void OnPreCull()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].enabled = false;
            }
        }
        void OnPostRender()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].enabled = true;
            }
        }
    }
}