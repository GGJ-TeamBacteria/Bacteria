/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class MixCast
    {
        public const string WEBSITE_URL = "http://blueprinttools.com/mixcast";
        public const string VERSION_STRING = "1.5.2";

        public static bool Active { get; protected set; }

        public static event System.Action MixCastEnabled;
        public static event System.Action MixCastDisabled;


        public static MixCastData Settings { get; protected set; }
        public static MixCastData.SecureData SecureSettings { get; protected set; }
        public static MixCastData.CameraCalibrationData DisplayingCamera { get; set; }
        public static List<MixCastData.CameraCalibrationData> RecordingCameras { get; protected set; }
        public static List<MixCastData.CameraCalibrationData> StreamingCameras { get; protected set; }

        static MixCast()
        {
            Settings = MixCastRegistry.ReadData();
            SecureSettings = MixCastRegistry.ReadSecureData();
            DisplayingCamera = null;
            RecordingCameras = new List<MixCastData.CameraCalibrationData>();
            StreamingCameras = new List<MixCastData.CameraCalibrationData>();

            if (!TamperDetection.CheckSDK()) {
                MixCastToggle.CanToggleOn = false;
            }

            if (Settings.global.startAutomatically)
                SetActive(true);
        }

        public static void SetActive(bool active)
        {
            if (Active == active)
                return;

            Active = active;
            if (Active)
            {
                if( MixCastEnabled != null )
                    MixCastEnabled();

                if (Settings.cameras.Count > 0)
                    DisplayingCamera = Settings.cameras[0];
            }
            else
            {
                if( MixCastDisabled != null )
                    MixCastDisabled();
            } 
        }

        //Returns true if compareTo is a later version than current
        public static bool IsVersionLater(string current, string compareTo)
        {
            if (current == compareTo)
                return false;

            string[] compareToNums = compareTo.Split('.');
            string[] currentNums = current.Split('.');

            for (int i = 0; i < compareToNums.Length && i < currentNums.Length; i++)
            {
                try {
                    int compareToNum = int.Parse(compareToNums[i]);
                    int currentNum = int.Parse(currentNums[i]);

                    if (compareToNum > currentNum)
                        return true;
                    else if (compareToNum < currentNum)
                        return false;

                } catch (FormatException) {
                    Debug.LogError("version check failed due to invalid string format");
                    return false;
                }
            }

            if (compareToNums.Length > currentNums.Length)
                return true;

            return false;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("MixCast/Go to Website")]
        public static void GoToWebsite()
        {
            Application.OpenURL(WEBSITE_URL);
        }
#endif
    }
}