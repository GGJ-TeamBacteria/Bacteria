using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public static class MixCastFiles
    {
        public const string FOLDER_NAME = "MixCast";
        private const string COPY_FILE_PATTERN = "{0} ({1})";

        public static string GetOutputDirectory()
        {
            if (string.IsNullOrEmpty(MixCast.Settings.global.rootOutputPath))
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), FOLDER_NAME).Replace("\\", "/");
            else
                return MixCast.Settings.global.rootOutputPath;
        }
        
        public static string GetApplicationFolderName()
        {
            return Application.productName;
        }

        public static void TryCreateFolderPath(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists)
                return;

            DirectoryInfo parentInfo = new DirectoryInfo(Path.GetDirectoryName(path));
            if (!parentInfo.Exists)
                TryCreateFolderPath(info.Parent.FullName);
            
            Directory.CreateDirectory(path);
        }

        public static string GenerateProceduralFilename()
        {
            return DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
        }
        public static string GetAvailableFilename(string origName)
        {
            if (!File.Exists(origName))
                return origName;

            string fileExt = Path.GetExtension(origName);
            string withoutExt = origName.Substring(0, origName.Length - fileExt.Length);
            int index = 1;
            while (true)
            {
                string newName = string.Format(COPY_FILE_PATTERN, withoutExt, index) + fileExt;
                if (!File.Exists(newName))
                    return newName;
                index++;
            }
        }
    }
}