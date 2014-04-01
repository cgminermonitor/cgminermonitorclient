using System;
using CgminerMonitorClient.CgminerMonitor.Common;

namespace CgminerMonitorClient
{
    public static class Consts
    {
        public static string Bamt1XDistroName = "BAMT1.x";
        public static string MacOsxDistroName = "MacOSX";
        public static string DefaultConfigFileName = "CgminerMonitorClient.config";

        public static string TroubleshooterResponse = "CgminerMonitor";

#if FORDEBUGGING
        public static string StatisticsUrl = "http://127.0.0.1:81/Statistics/";
        public static string TroubleshooterUrl = "http://127.0.0.1:81/Troubleshooter/";
        public static int RequestTimeoutInMiliseconds = (int)TimeSpan.FromDays(1).TotalMilliseconds;
        public static string ClientUpdateTemplateUrl = "http://127.0.0.1:81/ClientUpdates/{0}/{1}/{2}/{3}";
        public static TimeSpan FirstUpdateCheckSleepTime = TimeSpan.FromSeconds(3);
        public static TimeSpan NormalUpdateCheckSleepTime = TimeSpan.FromMinutes(3);
#endif
#if FORRELEASE
        public static string StatisticsUrl = "http://api.cgminermonitor.com/Statistics/";
        public static string TroubleshooterUrl = "http://api.cgminermonitor.com/Troubleshooter/";
        public static int RequestTimeoutInMiliseconds = (int)TimeSpan.FromSeconds(3).TotalMilliseconds;
        public static string ClientUpdateTemplateUrl = "http://api.cgminermonitor.com/ClientUpdates/{0}/{1}/{2}/{3}";
        public static TimeSpan FirstUpdateCheckSleepTime = TimeSpan.FromSeconds(30);
        public static TimeSpan NormalUpdateCheckSleepTime = TimeSpan.FromHours(3);
#endif

#if FORDEBUGGING
        public static string Distro = "test";
#elif BAMT1x
        public static string Distro = "BAMT1.x";
#elif MacOSX
        public static string Distro = "MacOSX";
#elif NORMAL
        public static string Distro = null;
#endif

        public static string GetUpdateUrl(ClientMetadata clientMetadata)
        {
            return string.Format(ClientUpdateTemplateUrl,
                (int) clientMetadata.ClientPlatform, (int) clientMetadata.ClientArchitecture, clientMetadata.ClientVersion, clientMetadata.Distro);
        }

        public static int HttpClientRetries = 5;
    }
}