/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class SetActiveFromKey : MonoBehaviour
    {
        public List<KeyCode> keys = new List<KeyCode>();
        public bool all = false;

        public List<GameObject> active = new List<GameObject>();
        public List<GameObject> inactive = new List<GameObject>();

        bool lastState;

        void OnEnable()
        {
            SetState(CalculateNewState());
        }
        void Update()
        {
            bool newState = CalculateNewState();
            if (newState != lastState)
                SetState(newState);
        }

        bool CalculateNewState()
        {
            if (keys.Count == 0)
                return false;

            if (all)
            {
                for (int i = 0; i < keys.Count; i++)
                    if (!Input.GetKey(keys[i]))
                        return false;
                return true;
            }
            else
            {
                for (int i = 0; i < keys.Count; i++)
                    if (Input.GetKey(keys[i]))
                        return true;
                return false;
            }
        }
        void SetState(bool newState)
        {
            lastState = newState;
            active.ForEach(g => g.SetActive(newState));
            inactive.ForEach(g => g.SetActive(!newState));
        }
    }
}
