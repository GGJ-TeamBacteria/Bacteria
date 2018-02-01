
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

using UnityEngine;

namespace BlueprintReality.MixCast
{
    [Serializable]
    public class TamperDetectionFileEntry
    {
        public string Path;
        public byte[] Hash;
    }

    public class TamperDetection
    {
#if UNITY_EDITOR
        private static string MixCastSDKRoot = @"\..\..\Assets\MixCast\";
#else
        private static string MixCastSDKRoot = @"\..\";
#endif

#if UNITY_EDITOR
        private static string MixCastStudioRoot = @"\..\..\";
#else
        private static string MixCastStudioRoot = @"\..\";
#endif

        private static List<TamperDetectionFileEntry> SDKFiles = new List<TamperDetectionFileEntry>()
        {
            new TamperDetectionFileEntry() {
                Path = MixCastSDKRoot + @"Scripts\Crypto\BouncyCastle.dll",
                Hash = Convert.FromBase64String("tDGyqbczdzjiX4KT3wsHDIiQuVQ=")
            }
        };

        private static List<TamperDetectionFileEntry> StudioFiles = new List<TamperDetectionFileEntry>()
        {
            new TamperDetectionFileEntry() {
                Path = MixCastStudioRoot + @"QlmWizard4\QlmLicenseWizard.exe",
                Hash = Convert.FromBase64String("35ZPJ/RwSA9vYON5P7Cfbf4C1d0=")
            },

            new TamperDetectionFileEntry() {
                Path = MixCastStudioRoot + @"QlmWizard4\QlmLicenseLib.dll",
                Hash = Convert.FromBase64String("IboY0Z84T791NgThpB71iIZax/Y=")
            }
        };

        private SHA1 _HashProvider;

        private string WorkingDir;

        public TamperDetection()
        {
            _HashProvider = SHA1.Create();

            WorkingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static bool CheckSDK()
        {
            TamperDetection td = new TamperDetection();
            return td.CheckFiles(SDKFiles);
        }

        public static bool CheckStudio()
        {
            TamperDetection td = new TamperDetection();
            return td.CheckFiles(StudioFiles);
        }

        private bool CheckFiles(List<TamperDetectionFileEntry> files)
        {
            foreach (var entry in files) {
                if (!CheckFileEntry(entry)) {
                    return false;
                }
            }

            return true;
        }

        private bool CheckFileEntry(TamperDetectionFileEntry entry)
        {
            var filePath = WorkingDir + entry.Path;

            if (!File.Exists(filePath)) {
                Debug.LogError("file not found: " + filePath);
                return false;
            }

            byte[] hash = _HashProvider.ComputeHash(File.OpenRead(filePath));
            bool result = MixCastCryptoUtils.BytesEqual(hash, entry.Hash);
            if (!result) {
#if UNITY_EDITOR
                Debug.LogError(string.Format("hash mismatch: {0}, expected {1} got {2}",
                    entry.Path, Convert.ToBase64String(entry.Hash), Convert.ToBase64String(hash)));
#else
                Debug.LogError("file change detected: " + filePath);
#endif
            }
            return result;
        }
    }
}
