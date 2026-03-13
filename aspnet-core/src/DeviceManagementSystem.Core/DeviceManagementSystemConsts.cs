using DeviceManagementSystem.Debugging;

namespace DeviceManagementSystem
{
    public class DeviceManagementSystemConsts
    {
        public const string LocalizationSourceName = "DeviceManagementSystem";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "a007ddc58d3b4bdfb0210b46a7444339";
    }
}
