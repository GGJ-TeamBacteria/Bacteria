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

namespace BlueprintReality.MixCast {
    public class FrameDelayQueue<T> {
        [System.Serializable]
        public class Frame<D>
        {
            public float timestamp;
            public D data;
        }

        public float delayDuration = 0;

        public List<Frame<T>> unusedFrames = new List<Frame<T>>();
        public List<Frame<T>> usedFrames = new List<Frame<T>>();

        public T OldestFrameData
        {
            get
            {
                if (usedFrames.Count > 0)
                    return usedFrames[0].data;
                else
                    return default(T);
            }
        }

        public void AllocateFrames()
        {
            int total = Mathf.Max(1, Mathf.CeilToInt(MixCast.Settings.global.targetFramerate * delayDuration));
            total -= usedFrames.Count;
            while (unusedFrames.Count < total)
                unusedFrames.Add(new Frame<T>());
        }

        public void Update()
        {
            while (usedFrames.Count > 1 && usedFrames[1].timestamp < Time.realtimeSinceStartup - delayDuration)
            {
                unusedFrames.Add(usedFrames[0]);
                usedFrames.RemoveAt(0);
            }
        }

        public Frame<T> GetNewFrame()
        {
            Frame<T> frame;
            if (unusedFrames.Count == 0)
                frame = new Frame<T>();
            else
            {
                frame = unusedFrames[unusedFrames.Count - 1];
                unusedFrames.RemoveAt(unusedFrames.Count - 1);
            }

            frame.timestamp = Time.realtimeSinceStartup;
            usedFrames.Add(frame);
            return frame;
        }
	}
}