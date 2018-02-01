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

using UnityEngine;
#if MIXCAST_STEAMVR
using Valve.VR;
#endif

namespace BlueprintReality.MixCast
{
    public class TrackingSpaceOrigin
    {
        /// <summary>
        /// Gets the difference between SteamVR and Oculus origin as a Position and Rotation data set.
        /// </summary>
        /// <returns>OculusOrigin offset data</returns>
        public static MixCastData.OculusOrigin GetOriginOffsetData()
        {
            /* Approach 1 works for setup when user Quick Calibrate Oculus in SteamVR.
             * It does not work when user do regular Room setup.
             * Quick Calibrate is under: SteamVR > Settings > Developer > Quick Calibrate
             * 
             * !!! HOWEVER, if we don't ping GetLiveSeatedZeroPoseToRawTrackingPose(), then the other
             * approach won't work, because GetRawZeroPoseToStandingAbsoluteTrackingPose() will return 
             * unitialized identity matrix for offset.
             */

            // APPROACH 1:
            // =========================================================

#if MIXCAST_STEAMVR
            HmdMatrix34_t offsetMatrix = new HmdMatrix34_t();

            if (OpenVR.ChaperoneSetup != null)
            {
                // This gets offset matrix to Seated position (but we want standing and there is no access to standing)
                OpenVR.ChaperoneSetup.GetLiveSeatedZeroPoseToRawTrackingPose(ref offsetMatrix);
            }

            //Debug.Log("GetLiveSeatedZeroPoseToRawTrackingPose: " + PrintMatrix(offsetMatrix));

            /* Approach 2 works for regular Room Setup. However when user do Room Setup first and then 
             * Quick Calibrate, it does not work. Looks like that both approaches has to be combined together.
             */

            // APPROACH 2:
            // =========================================================

            HmdMatrix34_t rawOffsetMatrix = new HmdMatrix34_t();

            if (OpenVR.System != null)
            {
                rawOffsetMatrix = OpenVR.System.GetRawZeroPoseToStandingAbsoluteTrackingPose();
            }

            //Debug.Log("GetRawZeroPoseToStandingAbsoluteTrackingPose: " + PrintMatrix(rawOffsetMatrix));

            // Convert to Unity Matrix and extract Position and Rotation.
            var unityMat = ConvertMatrixOpenVRToUnity(rawOffsetMatrix);

            var position = ExtractPosition(unityMat);
            position.x = -position.x;
            position.y = -position.y;

            var rotation = ExtractRotation(unityMat).eulerAngles;

            return new MixCastData.OculusOrigin()
            {
                position = position,
                rotation = rotation
            };
#else
            return new MixCastData.OculusOrigin();
#endif
        }

#if MIXCAST_STEAMVR
        public static string PrintMatrix(HmdMatrix34_t m)
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}", m.m0, m.m1, m.m2, m.m3, m.m4, m.m5, m.m6, m.m7, m.m8, m.m9, m.m10, m.m11);
        }

        public static Matrix4x4 ConvertMatrixOpenVRToUnity(HmdMatrix34_t m)
        {
            Matrix4x4 unityMat = new Matrix4x4();
            unityMat.SetRow(0, new Vector4(m.m0, m.m1, m.m2, m.m3));
            unityMat.SetRow(1, new Vector4(m.m4, m.m5, m.m6, m.m7));
            unityMat.SetRow(2, new Vector4(m.m8, m.m9, m.m10, m.m11));

            return unityMat;
        }

        public static HmdMatrix34_t ConvertMatrixUnityToOpenVR(Matrix4x4 m)
        {
            HmdMatrix34_t openvrMat = new HmdMatrix34_t();
            openvrMat.m0 = m[0, 0];
            openvrMat.m1 = m[0, 1];
            openvrMat.m2 = m[0, 2];
            openvrMat.m3 = m[0, 3];

            openvrMat.m4 = m[1, 0];
            openvrMat.m5 = m[1, 1];
            openvrMat.m6 = m[1, 2];
            openvrMat.m7 = m[1, 3];

            openvrMat.m8 = m[2, 0];
            openvrMat.m9 = m[2, 1];
            openvrMat.m10 = m[2, 2];
            openvrMat.m11 = m[2, 3];

            return openvrMat;
        }
#endif

        public static Vector3 ExtractPosition(Matrix4x4 matrix)
        {
            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }

        public static Quaternion ExtractRotation(Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        public static Vector3 ExtractScale(Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }
    }
}