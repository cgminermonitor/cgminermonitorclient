using System;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.CgminerMonitor.Common
{
    public class ClientMetadata
    {
        public ClientPlatform ClientPlatform { get; set; }
        public ClientArchitecture ClientArchitecture { get; set; }
        public string ClientVersion { get; set; } //like 1.0.0.0 (exactly 7 chars)

        public static ClientMetadata GetCurrentClientMetadata()
        {
            return new ClientMetadata
            {
                ClientArchitecture = IntPtr.Size == 8 ? ClientArchitecture.x64 : ClientArchitecture.x86,
                ClientPlatform = PlatformCheck.AreWeRunningUnderWindows() ? ClientPlatform.Windows : ClientPlatform.Linux,
                ClientVersion = VersionUtil.GetAsString(),
            };
        }

        public string Dump()
        {
            return string.Format("{0} {1} - {2}", ClientPlatform, ClientArchitecture, ClientVersion);
        }
    }
}