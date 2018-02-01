/**********************************************************************************
* Blueprint Reality Inc. CONFIDENTIAL
* 2017 Blueprint Reality Inc.
* All Rights Reserved.
*
* NOTICE:  All information contained herein is, and remains, the property of
* Blueprint Reality Inc. and its suppliers, if any.  The intellectual and
* technical concepts contained herein are proprietary to Blueprint Reality Inc.
* and its suppliers and may be covered by Patents, pending patents, and are
* protected by trade secret or copyright law.
*
* Dissemination of this information or reproduction of this material is strictly
* forbidden unless prior written permission is obtained from Blueprint Reality Inc.
***********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if MIXCAST_STEAMVR
using Valve.VR;
#endif

namespace BlueprintReality.MixCast {
	public partial class TrackedDeviceManager {
#if MIXCAST_OCULUS
        public bool GetDeviceTransformByGuid_Oculus(string guid, out Vector3 position, out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            return false;
        }
        public bool GetDeviceTransformByIndex_Oculus(int index, out Vector3 position, out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            return false;
        }
        public bool GetDeviceTransformByRole_Oculus(DeviceRole role, out Vector3 position, out Quaternion rotation)
        {
            OVRPlugin.Node node;
            switch(role)
            {
                case DeviceRole.Head:
                    node = OVRPlugin.Node.Head;
                    break;
                case DeviceRole.LeftHand:
                    node = OVRPlugin.Node.HandLeft;
                    break;
                case DeviceRole.RightHand:
                    node = OVRPlugin.Node.HandRight;
                    break;
                default:
                    position = Vector3.zero;
                    rotation = Quaternion.identity;
                    return false;
            }
            if (OVRPlugin.GetNodePositionTracked(node) && OVRPlugin.GetNodeOrientationTracked(node))
            {
                OVRPlugin.Posef pose = OVRPlugin.GetNodePose(node, OVRPlugin.Step.Render);
                position = new Vector3(pose.Position.x, pose.Position.y, pose.Position.z);
                rotation = new Quaternion(pose.Orientation.x, pose.Orientation.y, pose.Orientation.z, pose.Orientation.w);
                return true;
            }
            else
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return false;
            }
        }

        public void UpdateTransforms_Oculus()
        {
            if (OnTransformsUpdated != null)
                OnTransformsUpdated();
        }
#endif
    }
}