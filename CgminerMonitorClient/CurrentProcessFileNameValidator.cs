using System;
using System.Diagnostics;
using System.IO;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient
{
    internal class CurrentProcessFileNameValidator
    {
        private const string WarningTemplate = "WARNING!!! It is highly recommended to leave oryginal '{0}' executable name (due to future autoupdate problems). Your current executable name is '{1}'.";

        public void Validate()
        {
            var fileName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
            var currentClientMetadata = ClientMetadata.GetCurrentClientMetadata();
            switch (currentClientMetadata.ClientPlatform)
            {
                case ClientPlatform.Linux:
                    ValidateLinuxBinaryName(fileName, currentClientMetadata.ClientArchitecture);
                    break;
                case ClientPlatform.Windows:
                    ValidateWindowsBinaryName(fileName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ValidateLinuxBinaryName(string fileName, ClientArchitecture clientArchitecture)
        {
            var x86LinuxName = string.IsNullOrEmpty(Consts.Distro) ? "CgminerMonitorClientX86Linux" : "CgminerMonitorClientX86" + Consts.Distro.Replace(".", "");
            var x64LinuxName = string.IsNullOrEmpty(Consts.Distro) ? "CgminerMonitorClientX64Linux" : "CgminerMonitorClientX64" + Consts.Distro.Replace(".", "");
            switch (clientArchitecture)
            {
                case ClientArchitecture.x64:
                    if (string.CompareOrdinal(fileName, x64LinuxName) != 0)
                        Log.Instance.InfoFormat(WarningTemplate, x64LinuxName, fileName);
                    break;
                case ClientArchitecture.x86:
                    if (string.CompareOrdinal(fileName, x86LinuxName) != 0)
                        Log.Instance.InfoFormat(WarningTemplate, x86LinuxName, fileName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("clientArchitecture");
            }
        }

        private static void ValidateWindowsBinaryName(string fileName)
        {
            const string cgminermonitorclientWindowsExe = "CgminerMonitorClient_Windows.exe";
            if (string.CompareOrdinal(fileName, cgminermonitorclientWindowsExe) != 0)
                Log.Instance.InfoFormat(WarningTemplate, cgminermonitorclientWindowsExe, fileName);
        }
    }
}