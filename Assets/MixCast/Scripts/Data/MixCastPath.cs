
using System;

namespace BlueprintReality.MixCast
{
    public class MixCastPath
    {
        private static string BlueprintRealityMixCastVR
        {
            get {
                return @"Blueprint Reality\MixCast VR";
            }
        }

        public static string Registry
        {
            get {
                return @"SOFTWARE\" + BlueprintRealityMixCastVR;
            }
        }

        public static string MyDocuments
        {
            get {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + BlueprintRealityMixCastVR;
            }
        }

        public static string LocalApplicationData
        {
            get {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + BlueprintRealityMixCastVR;
            }
        }
    }
}
