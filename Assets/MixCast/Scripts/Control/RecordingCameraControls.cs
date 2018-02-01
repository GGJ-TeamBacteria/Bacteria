﻿/**********************************************************************************
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

namespace BlueprintReality.MixCast {
	public class RecordingCameraControls : MonoBehaviour {
        public void StartCameraRecording(CameraConfigContext camera)
        {
            if( camera.Data != null )
                MixCast.RecordingCameras.Add(camera.Data);
        }
        public void StopCameraRecording(CameraConfigContext camera)
        {
            if (camera.Data != null)
                MixCast.RecordingCameras.Remove(camera.Data);
        }
        public void ToggleCameraRecording(CameraConfigContext camera)
        {
            if (camera.Data == null)
                return;
            if (MixCast.RecordingCameras.Contains(camera.Data))
                MixCast.RecordingCameras.Remove(camera.Data);
            else
                MixCast.RecordingCameras.Add(camera.Data);
        }
	}
}