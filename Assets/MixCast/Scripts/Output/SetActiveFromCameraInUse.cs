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
	public class SetActiveFromCameraInUse : CameraComponent {
        public bool displaying = true;
        public bool recording = true;
        public bool streaming = true;

        public List<GameObject> activeInUse = new List<GameObject>();
        public List<GameObject> inactiveInUse = new List<GameObject>();

        private bool lastState;

        protected override void OnEnable()
        {
            base.OnEnable();
            SetNewState(CalculateNewState());
        }
        private void Update()
        {
            bool newState = CalculateNewState();
            if (newState != lastState)
                SetNewState(newState);
        }

        bool CalculateNewState()
        {
            if (context.Data == null)
                return false;

            if (displaying && MixCast.DisplayingCamera == context.Data)
                return true;
            if (recording && MixCast.RecordingCameras.Contains(context.Data))
                return true;
            if (streaming && MixCast.StreamingCameras.Contains(context.Data))
                return true;
            return false;
        }
        void SetNewState(bool newState)
        {
            lastState = newState;
            activeInUse.ForEach(g => g.SetActive(newState));
            inactiveInUse.ForEach(g => g.SetActive(!newState));
        }
    }
}