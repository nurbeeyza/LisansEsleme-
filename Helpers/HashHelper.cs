using System.Security.Cryptography;
using System.Text;

namespace LisansEşlemeUyg.Helpers
{
    public static class HashHelper
    {
        public static string ComputeFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    var hashBytes = sha256.ComputeHash(fileStream);
                    return Convert.ToBase64String(hashBytes);
                }
            }
        }

        public static bool CompareHash(string computedHash, string storedHash)
        {
            return string.Equals(computedHash, storedHash, StringComparison.Ordinal);
        }
    }
}
