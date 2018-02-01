/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    public class RotateRoomControls : MonoBehaviour
    {
        public float angleStep = 90f;

        // clockwise variables
        public KeyCode cwKey = KeyCode.LeftArrow;
        public KeyCode cwModifier1 = KeyCode.None;
        public KeyCode cwModifier2 = KeyCode.None;

        // counter-clockwise variables
        public KeyCode ccwKey = KeyCode.RightArrow;
        public KeyCode ccwModifier1 = KeyCode.None;
        public KeyCode ccwModifier2 = KeyCode.None;

        void Update()
        {
            if (Selectable.allSelectables.Find(s => s is InputField && (s as InputField).isFocused) != null)
            {
                return;
            }

            /*
             * SET CLOCKWISE KEYS
             * */
            bool goCw = Input.GetKeyDown(cwKey);
            bool goCWModBoth = goCw;

            // if one modifier key is pressed
            if (cwModifier1 != KeyCode.None)
            {
                goCw &= Input.GetKey(cwModifier1);
                // case to capture when both modifier keys are pressed
                if (cwModifier2 != KeyCode.None)
                {
                    goCWModBoth &= Input.GetKey(cwModifier2);
                }

            }

            // if other modifier key is pressed
            else if (cwModifier2 != KeyCode.None)
            {
                goCw &= Input.GetKey(cwModifier2);
            }

            /*
             * SET COUNTER-CLOCKWISE KEYS
             * */
            bool goCcw = Input.GetKeyDown(ccwKey);
            bool goCcWModBoth = goCcw;

            // if one modifier key is pressed
            if (ccwModifier1 != KeyCode.None)
            {
                goCcw &= Input.GetKey(ccwModifier1);
                // case to capture when both modifier keys are pressed
                if (ccwModifier2 != KeyCode.None)
                {
                    goCcWModBoth &= Input.GetKey(ccwModifier2);
                }

            }

            // if other modifier key is pressed
            else if (ccwModifier2 != KeyCode.None) // else if for either key not both
            {
                goCcw &= Input.GetKey(ccwModifier2);
            }


            /*
             * SET ROOM ROTATION BASED ON KEYS PRESSED
             * */
            if (goCw || goCWModBoth)
            {
                Debug.Log("rotate clockwise");

                // this is the case for MixCast Studio Main scene, where main camera (eye) is a direct child of camera rig
#if MIXCAST_STEAMVR
                if (Camera.main.transform.parent.GetComponent<SteamVR_TrackedObject>() == null)
                {
                    Camera.main.transform.parent.Rotate(0, angleStep, 0);
                }
                // this is the case for the SteamVR prefab, where main camera (eye) is a child of head camera, which is child of camera rig
                else
                {
                    Camera.main.transform.parent.parent.Rotate(0, angleStep, 0);
                }
#else
                Camera.main.transform.parent.parent.Rotate(0, angleStep, 0);
#endif
            }

            else if (goCcw || goCcWModBoth)
            {
                Debug.Log("rotate counter-clockwise");

                /*
                 * this is the case for MixCast Studio Main scene, where main camera (eye) is a direct child of camera rig
                 * SteamVR_TrackedObject component should not be present on the camerar rig object
                 * Alternatively, could check fo rthe SteamVR_ControllerManager or SteamVR_PlayArea component, which should be on the camera rig but not on the camera (head) child
                 * */
#if MIXCAST_STEAMVR
                if (Camera.main.transform.parent.GetComponent<SteamVR_TrackedObject>() == null)
                {
                    Camera.main.transform.parent.Rotate(0, -angleStep, 0);
                }
                // this is the case for the SteamVR prefab, where main camera (eye) is a child of head camera, which is child of camera rig
                else
                {
                    Camera.main.transform.parent.parent.Rotate(0, -angleStep, 0);
                }
#else
                Camera.main.transform.parent.parent.Rotate(0, angleStep, 0);
#endif
            }

        }
    }
}