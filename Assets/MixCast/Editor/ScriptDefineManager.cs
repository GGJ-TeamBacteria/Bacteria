/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Class that handles enforcing the script defines on project necessary for appropriate SDK interaction
namespace BlueprintReality.MixCast
{
    [InitializeOnLoad]
    public class ScriptDefineManager
    {
        public const string STEAMVR_DEFINE = "MIXCAST_STEAMVR";
        public const string STEAMVR_FILE = "SteamVR.cs";    //Just a file that indicates the SDK's inclusion

        public const string OCULUS_DEFINE = "MIXCAST_OCULUS";
        public const string OCULUS_FILE = "OVRManager.cs";

        static ScriptDefineManager()
        {
            EnforceAppropriateScriptDefines();
        }

        public static bool EnforceAppropriateScriptDefines()
        {
            string defineStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            List<string> defineList = new List<string>(defineStr.Split(';'));

            bool changedSteamVr = EnforceLibraryFlag(STEAMVR_DEFINE, STEAMVR_FILE, defineList);
            bool changedOculus = EnforceLibraryFlag(OCULUS_DEFINE, OCULUS_FILE, defineList);

            bool changedDefines = changedSteamVr || changedOculus;
            if (changedDefines)
            {
                defineStr = string.Join(";", defineList.ToArray());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), defineStr);

                if (changedSteamVr && defineList.Contains(STEAMVR_DEFINE))
                    Debug.Log("Enabled MixCast SteamVR Support");
                if (changedOculus && defineList.Contains(OCULUS_DEFINE))
                    Debug.Log("Enabled MixCast Oculus Support");
            }

            return changedDefines;
        }

        //Returns true if the define list has been modified
        static bool EnforceLibraryFlag(string libraryFlag, string libraryIdentifier, List<string> currentDefines)
        {
            bool libraryFound = System.IO.Directory.GetFiles(Application.dataPath, libraryIdentifier, System.IO.SearchOption.AllDirectories).Length > 0;
            bool modifying = currentDefines.Contains(libraryFlag) != libraryFound;
            if (modifying)
            {
                if (libraryFound)
                    currentDefines.Add(libraryFlag);
                else
                    currentDefines.Remove(libraryFlag);
            }
            return modifying;
        }
    }
}