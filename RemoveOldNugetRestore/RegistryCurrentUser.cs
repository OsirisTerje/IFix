using Microsoft.Win32;

namespace IFix
{
    public class RegistryCurrentUser : RegistryBase
    {
        public RegistryCurrentUser(string subkeyname)
            : base(@"HKEY_CURRENT_USER\", subkeyname)
        {

        }

        private static RegistryCurrentUser currentUser;

        public static RegistryCurrentUser OpenRegistryCurrentUser(string key)
        {
            return currentUser ?? (currentUser = new RegistryCurrentUser(key));
        }

        /// <summary>
        /// Used for test mocking only
        /// </summary>
        /// <param name="user"></param>
        public void SetCurrentUser(RegistryCurrentUser user)
        {
            currentUser = user;
        }

        public override bool ExistKey()
        {
            var res = Registry.CurrentUser.OpenSubKey(SubKeyName);
            return res != null;
        }
    }
}