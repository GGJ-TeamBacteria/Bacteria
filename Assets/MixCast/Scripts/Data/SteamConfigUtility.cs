using BlueprintReality.MixCast;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using BlueprintReality;
using System;

namespace BlueprintReality.MixCast
{
    public static class SteamConfigUtility
    {
        public const string CFG_FILENAME = "externalcamera.cfg";
        
        //returns the generated config file path
        public static string Run(string exePath)
        {
            string dataFilePath = GetSrcPath();
            if (!File.Exists(dataFilePath))
            {
                Debug.LogError("MixCast VR Studio data file not found");
                return null;
            }
            string txt = File.ReadAllText(dataFilePath);

            string targetFolderPath = Path.GetDirectoryName(exePath);
            string targetFilePath = Path.Combine(targetFolderPath, CFG_FILENAME);

            File.WriteAllText(targetFilePath, txt);
            return targetFilePath;
        }

        public static bool ShouldRun(string exePath)
        {
            string dst = GetDstPath(exePath);
            if (!File.Exists(dst))
                return true;
            string src = GetSrcPath();
            string srcTxt = File.ReadAllText(src);
            string dstTxt = File.ReadAllText(dst);
            return srcTxt != dstTxt;
        }

        static string GetSrcPath()
        {
            string dataFolderPath = Application.persistentDataPath;
            dataFolderPath = Directory.GetParent(dataFolderPath).FullName;
            dataFolderPath = Path.Combine(dataFolderPath, "MixCast Studio");
            return Path.Combine(dataFolderPath, CFG_FILENAME);
        }
        static string GetDstPath(string exePath)
        {
            string targetFolderPath = Path.GetDirectoryName(exePath);
            return Path.Combine(targetFolderPath, CFG_FILENAME);
        }
    }
}