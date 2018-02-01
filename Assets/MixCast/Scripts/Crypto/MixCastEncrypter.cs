
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;

namespace BlueprintReality.MixCast
{
    public class MixCastEncrypter
    {
        private Pkcs1Encoding _Cipher;

        public MixCastEncrypter(byte[] publicKey)
        {
            Init(publicKey);
        }

        public void Init(byte[] publicKey)
        {
            var key = PublicKeyFactory.CreateKey(publicKey);
            _Cipher = new Pkcs1Encoding(new RsaEngine());
            _Cipher.Init(true, key);
        }

        public byte[] Encrypt(byte[] data)
        {
            return MixCastCryptoUtils.ProcessCipher(data, _Cipher);
        }
    }
}
