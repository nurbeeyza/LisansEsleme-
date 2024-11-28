using System.IO;

namespace LisansEşlemeUyg.Helpers
{
    public static class KeyHelper
    {
        public static string ReadPublicKey()
        {
            return File.ReadAllText("C:\\Anahtar\\publicKey.txt");
        }

        public static string ReadPrivateKey()
        {
            return File.ReadAllText("C:\\Anahtar\\privateKey.txt");
        }

        public static string ReadEncryptedData()
        {
            return File.ReadAllText("C:\\Anahtar\\encryptedData.txt");
        }
    }
}
