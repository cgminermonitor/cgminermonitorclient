using System;

namespace CgminerMonitorClient.Utils
{
    public static class PlatformCheck
    {
        public static bool AreWeRunningUnderWindows()
        {
            var os = Environment.OSVersion;
            var pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return true;
                case PlatformID.Unix:
                    return false;
                default:
                    return false;
            }
        }
    }
}