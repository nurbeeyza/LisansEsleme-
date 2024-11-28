using Microsoft.Win32;

namespace LisansEşlemeUyg.Helpers
{
    public static class RegeditHelper
    {
        private const string RegistryPath = "SOFTWARE\\LisansEslemeUyg";

        public static void SaveKey(string keyName, string keyValue)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath))
            {
                key?.SetValue(keyName, keyValue);
            }
        }

        public static string ReadKey(string keyName)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                return key?.GetValue(keyName) as string;
            }
        }

        public static bool KeyExists(string keyName)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                return key?.GetValue(keyName) != null;
            }
        }
    }
}
