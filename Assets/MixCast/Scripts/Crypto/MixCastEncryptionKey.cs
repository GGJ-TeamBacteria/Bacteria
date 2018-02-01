
using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace BlueprintReality.MixCast
{
    public class MixCastEncryptionKey
    {
        private const int KeyLengthBits = 2048;

        public byte[] PrivateKey;
        public byte[] PublicKey;

        private static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            using (RSACryptoServiceProvider cryptoProvider = new RSACryptoServiceProvider(KeyLengthBits))
            {
                try {
                    return DotNetUtilities.GetRsaKeyPair(cryptoProvider.ExportParameters(true));
                } finally {
                    cryptoProvider.PersistKeyInCsp = false;
                }
            }
        }

        public void Generate()
        {
            var keyPair = GenerateKeyPair();

            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            PrivateKey = privateKeyInfo.GetDerEncoded();

            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
            PublicKey = publicKeyInfo.GetDerEncoded();
        }

        public static bool KeysAreCompatible(byte[] publicKey, byte[] privateKey)
        {
            if (publicKey == null || privateKey == null) { return false; }

            MixCastEncrypter encrypter = new MixCastEncrypter(publicKey);
            MixCastDecrypter decrypter = new MixCastDecrypter(privateKey);

            byte[] testData = { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9 };
            byte[] encrypted = encrypter.Encrypt(testData);
            byte[] decrypted = decrypter.Decrypt(encrypted);

            if (!MixCastCryptoUtils.BytesEqual(testData, decrypted)) {
                return false;
            }

            return true;
        }

        public static byte[] ReadKeyFromFile(string filePath)
        {
            return File.Exists(filePath) ? File.ReadAllBytes(filePath) : null;
        }

        public static byte[] ReadKeyFromRegistry(string registryKey)
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(MixCastPath.Registry);
            string keyStr = reg.GetValue(registryKey, null) as string;
            if (string.IsNullOrEmpty(keyStr)) { return null; }
            return Convert.FromBase64String(keyStr);
        }
    }
}
