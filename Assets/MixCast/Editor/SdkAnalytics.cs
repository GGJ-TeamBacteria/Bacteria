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
#define ALLOW_ANALYTICS     // <---- Feel free to disable if preferred

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace BlueprintReality.MixCast {
#if UNITY_EDITOR
    [InitializeOnLoad]
	public static class SdkAnalytics {
        const string URL = "https://analytics.blueprintreality.com/submit.php";
        const string PREF_KEY = "mixcast_lastSubmit_{0}";

        private static WWW sent;

        static SdkAnalytics()
        {
#if ALLOW_ANALYTICS
            string projectPrefKey = string.Format(PREF_KEY, PlayerSettings.productName);
            string lastDate = EditorPrefs.GetString(projectPrefKey, "");
            string newDate = DateTime.UtcNow.ToShortDateString();   //Max once per day
            if (lastDate != newDate)
            {
                WWWForm wwwForm = new WWWForm();
                PopulateData(wwwForm);
                sent = new WWW(URL, wwwForm);
                EditorPrefs.SetString(projectPrefKey, newDate);
            }
            EditorApplication.update += Update;
#endif
        }

        static void Update()
        {
            if( sent != null && sent.isDone )
                sent = null;
        }

        static void PopulateData(WWWForm form)
        {
            form.AddField("eventID", "sdkActive");

            form.AddField("orgName", PlayerSettings.companyName);
            form.AddField("projectName", PlayerSettings.productName);
            form.AddField("uid", GetMacAddress());
            form.AddField("unityVer", Application.unityVersion);
            form.AddField("mixcastSDK", MixCast.VERSION_STRING);
        }

        static string GetMacAddress()
        {
            List<NetworkInterface> networks = new List<NetworkInterface>(NetworkInterface.GetAllNetworkInterfaces());
            var activeNetworks = networks.FindAll(ni => ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);
            activeNetworks.Sort((nix, niy) => { return -nix.Speed.CompareTo(niy.Speed); });
            return activeNetworks[0].GetPhysicalAddress().ToString();
        }
	}
#endif
}