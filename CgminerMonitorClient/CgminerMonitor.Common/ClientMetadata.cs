using System;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.CgminerMonitor.Common
{
    public class ClientMetadata
    {
        public ClientPlatform ClientPlatform { get; set; }
        public ClientArchitecture ClientArchitecture { get; set; }
        public string ClientVersion { get; set; } //like 1.0.0.0 (exactly 7 chars)
        public string Distro { get; set; }
        public string OsVersion { get; set; }

        public static ClientMetadata GetCurrentClientMetadata()
        {
            var result = new ClientMetadata
            {
                ClientArchitecture = IntPtr.Size == 8 ? ClientArchitecture.x64 : ClientArchitecture.x86,
                ClientPlatform = PlatformCheck.AreWeRunningUnderWindows() ? ClientPlatform.Windows : ClientPlatform.Linux,
                ClientVersion = VersionUtil.GetAsString(),
                Distro = Consts.Distro
            };
            try
            {
                result.OsVersion = Environment.OSVersion.ToString();
            }
            catch (Exception)
            {
                result.OsVersion = "unknown";
            }
            return result;
        }

        public string Dump()
        {
            if (!string.IsNullOrEmpty(Distro))
                return string.Format("{0} {1} {2} - {3}", ClientPlatform, Distro, ClientArchitecture, ClientVersion);
            return string.Format("{0} {1} - {2}", ClientPlatform, ClientArchitecture, ClientVersion);
        }
    }
}