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

using BlueprintReality.MixCast;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast {
    //This component exposes controller functions for modifying the actively displaying MixCast camera
	public class DisplayingCameraControls : MonoBehaviour {
        public void SetToFirstAvailable()
        {
            if (MixCast.Settings.cameras.Count > 0)
                MixCast.DisplayingCamera = MixCast.Settings.cameras[0];
        }
        public void SetToLastAvailable()
        {
            if (MixCast.Settings.cameras.Count > 0)
                MixCast.DisplayingCamera = MixCast.Settings.cameras[MixCast.Settings.cameras.Count - 1];
        }
        public void SetToConfig(CameraConfigContext context)
        {
            MixCast.DisplayingCamera = context.Data;
        }
        public void ClearDisplayed()
        {
            MixCast.DisplayingCamera = null;
        }

        public void CycleCameraForward()
        {
            if (MixCast.Settings.cameras.Count == 0)
                return;
            int curIndex = MixCast.Settings.cameras.IndexOf(MixCast.DisplayingCamera);
            if (curIndex == -1)
            {
                if(MixCast.Settings.cameras.Count > 0)
                    MixCast.DisplayingCamera = MixCast.Settings.cameras[0];
                return;
            }
            int camIndex = MixCast.Settings.cameras.IndexOf(MixCast.DisplayingCamera);
            camIndex++;
            if (camIndex >= MixCast.Settings.cameras.Count)
                camIndex = 0;
            MixCast.DisplayingCamera = MixCast.Settings.cameras[camIndex];
        }
        public void CycleCameraBackward()
        {
            if (MixCast.Settings.cameras.Count == 0)
                return;
            int curIndex = MixCast.Settings.cameras.IndexOf(MixCast.DisplayingCamera);
            if (curIndex == -1)
            {
                if (MixCast.Settings.cameras.Count > 0)
                    MixCast.DisplayingCamera = MixCast.Settings.cameras[0];
                return;
            }
            int camIndex = MixCast.Settings.cameras.IndexOf(MixCast.DisplayingCamera);
            camIndex--;
            if (camIndex < 0)
                camIndex = MixCast.Settings.cameras.Count - 1;
            MixCast.DisplayingCamera = MixCast.Settings.cameras[camIndex];
        }
    }
}