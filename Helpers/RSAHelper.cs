using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace LisansEşlemeUyg.Helpers
{
    public static class RSAHelper
    {
        public static (bool isValid, bool isKeyMismatch) ValidateData(string encryptedData, string expectedDecryptedData, ILogger logger)
        {
            string privateKey = KeyHelper.ReadPrivateKey();

            logger.LogInformation($"Private Key: {privateKey}");
            logger.LogInformation($"Encrypted Data: {encryptedData}");

            if (!IsValidPrivateKey(privateKey))
            {
                logger.LogError("Geçersiz private key formatı.");
                return (false, false);
            }

            if (!IsBase64String(encryptedData))
            {
                logger.LogError("Encrypted data geçersiz bir formatta (Base64 değil).");
                return (false, false);
            }

            using (var rsa = RSA.Create())
            {
                try
                {
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

                    var decryptedBytes = rsa.Decrypt(Convert.FromBase64String(encryptedData), RSAEncryptionPadding.Pkcs1);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);

                    logger.LogInformation($"Decrypted Data: {decryptedData}");
                    logger.LogInformation($"Expected Data: {expectedDecryptedData}");

                    if (decryptedData == expectedDecryptedData)
                    {
                        return (true, false);
                    }
                    else
                    {
                        return (false, true);
                    }
                }
                catch (CryptographicException ex)
                {
                    logger.LogError(ex, "Şifre çözme hatası: {Message}", ex.Message);
                    return (false, false);
                }
            }
        }

        private static bool IsValidPrivateKey(string privateKey)
        {
            try
            {
                using (var rsa = RSA.Create())
                {
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
    }
}
