/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/

using Microsoft.Win32;
using System;
using System.Text;

using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class MixCastRegistry
    {
        public const string SETTINGS_REGISTRY_KEY = "DATA";
        public const string SECURE_REGISTRY_KEY = "SETUP";
        public const string DECRYPT_REGISTRY_KEY = "HASH";

        private static MixCastDecrypter _Decrypt;

        public static void InitDecrypt()
        {
            byte[] privateKey = MixCastEncryptionKey.ReadKeyFromRegistry(DECRYPT_REGISTRY_KEY);
            _Decrypt = privateKey != null ? new MixCastDecrypter(privateKey) : null;
        }

        public static MixCastData ReadData()
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(MixCastPath.Registry);
            string dataStr = reg.GetValue(SETTINGS_REGISTRY_KEY, null) as string;
            MixCastData data = string.IsNullOrEmpty(dataStr) ? new MixCastData() : JsonUtility.FromJson<MixCastData>(dataStr);

            if (data.sourceVersion != MixCast.VERSION_STRING) {
                Debug.LogWarning(string.Format("settings version mis-match detected: {0} instead of {1}", data.sourceVersion, MixCast.VERSION_STRING));
                MixCastDataUtility.UpdateForBackwardCompatibility(data, dataStr);
            }

            return data;
        }

        public static MixCastData.SecureData ReadSecureData()
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(MixCastPath.Registry);
            MixCastData.SecureData data = ReadSecureSettingsFromRegistry(reg);

            if (data == null) {
                data = new MixCastData.SecureData();
            } else {
                MachineId thisComputerId = new MachineId();
                if (!IsSameComputer(data, thisComputerId))
                {
                    Debug.LogError("machine identifier mismatch detected");
                    if (data != null)
                    {
                        data.licenseType = MixCastData.LicenseType.Free;
                    }
                }
            }

            return data;
        }

        public static void WriteData(MixCastData data)
        {
            string dataStr = JsonUtility.ToJson(data);
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(MixCastPath.Registry);
            reg.SetValue(MixCastRegistry.SETTINGS_REGISTRY_KEY, dataStr);
        }

        private static MixCastData.SecureData ReadSecureSettingsFromRegistry(RegistryKey reg)
        {
            InitDecrypt();

            if (_Decrypt == null) { return null; }

            string encryptedDataStr = reg.GetValue(SECURE_REGISTRY_KEY, null) as string;
            if (string.IsNullOrEmpty(encryptedDataStr)) {
                Debug.LogWarning("no secure settings data found");
                return null;
            }

            byte[] encryptedData = Convert.FromBase64String(encryptedDataStr);
            byte[] decryptedData =_Decrypt.Decrypt(encryptedData);
            var dataStr = Encoding.UTF8.GetString(decryptedData);

            return string.IsNullOrEmpty(dataStr) ? null : JsonUtility.FromJson<MixCastData.SecureData>(dataStr);
        }

        private static bool IsSameComputer(MixCastData.SecureData data, MachineId machineId)
        {
            if (machineId == null || data == null || string.IsNullOrEmpty(data.machineId)) {
                return false;
            }

            return MixCastCryptoUtils.BytesEqual(
                    machineId.ComputeHash(),
                    Convert.FromBase64String(data.machineId));
        }
    }
}
