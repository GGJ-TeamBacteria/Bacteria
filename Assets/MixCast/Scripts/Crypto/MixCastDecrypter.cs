
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;

namespace BlueprintReality.MixCast
{
    public class MixCastDecrypter
    {
        private Pkcs1Encoding _Cipher;

        public MixCastDecrypter(byte[] privateKey)
        {
            Init(privateKey);
        }

        public void Init(byte[] privateKey)
        {
            var key = PrivateKeyFactory.CreateKey(privateKey);
            _Cipher = new Pkcs1Encoding(new RsaEngine());
            _Cipher.Init(false, key);
        }

        public byte[] Decrypt(byte[] data)
        {
            return MixCastCryptoUtils.ProcessCipher(data, _Cipher);
        }
    }
}
