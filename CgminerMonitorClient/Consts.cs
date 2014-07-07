using System;
using CgminerMonitorClient.CgminerMonitor.Common;

namespace CgminerMonitorClient
{
    public static class Consts
    {
        public static string Pimp1XDistroName = "PiMP1.x";
        public static string Bamt1XDistroName = "BAMT1.x";
        public static string MacOsxDistroName = "MacOSX";

        public static string DefaultConfigFileName = "CgminerMonitorClient.config";

        public static string TroubleshooterResponse = "CgminerMonitor";

#if FORDEBUGGING
        public static string StatisticsUrl = "http://127.0.0.1:81/Statistics/";
        public static string RemoteManagementUrl = "http://127.0.0.1:81/Control/";
        public static string TroubleshooterUrl = "http://127.0.0.1:81/Troubleshooter/";
        public static int RequestTimeoutInMiliseconds = (int)TimeSpan.FromDays(1).TotalMilliseconds;
        public static string ClientUpdateCheckTemplateUrl = "http://127.0.0.1:81/ClientUpdatesCheck/{0}/{1}/{2}/{3}";
        public static TimeSpan FirstUpdateCheckSleepTime = TimeSpan.FromSeconds(3);
        public static TimeSpan NormalUpdateCheckSleepTime = TimeSpan.FromMinutes(3);
#endif
#if FORRELEASE
        public static string StatisticsUrl = "http://api.cgminermonitor.com/Statistics/";
        public static string RemoteManagementUrl = "http://api.cgminermonitor.com/Control/";
        public static string TroubleshooterUrl = "http://api.cgminermonitor.com/Troubleshooter/";
        public static int RequestTimeoutInMiliseconds = (int)(TimeSpan.FromSeconds(3).TotalMilliseconds + TimeSpan.FromMilliseconds(500).TotalMilliseconds);
        public static string ClientUpdateCheckTemplateUrl = "http://api.cgminermonitor.com/ClientUpdatesCheck/{0}/{1}/{2}/{3}";
        public static TimeSpan FirstUpdateCheckSleepTime = TimeSpan.FromSeconds(30);
        public static TimeSpan NormalUpdateCheckSleepTime = TimeSpan.FromHours(3);
#endif

#if FORDEBUGGING
        public static string Distro = "test";
#elif BAMT1x
        public static string Distro = "BAMT1.x";
#elif PiMP1x
        public static string Distro = "PiMP1.x";
#elif Custom
        public static string Distro = "Custom";
#elif MacOSX
        public static string Distro = "MacOSX";
#elif NORMAL
        public static string Distro = null;
#endif

        public static string GetUpdateCheckUrl(ClientMetadata clientMetadata)
        {
            return string.Format(ClientUpdateCheckTemplateUrl,
                (int) clientMetadata.ClientPlatform, (int) clientMetadata.ClientArchitecture, clientMetadata.ClientVersion, clientMetadata.Distro);
        }

        public static int HttpClientRetries = 5;

        public static string SuccessCommandPrefix = "Success! ";
        public static string CgminerCommandTemplate = @"{{ ""command"": ""{0}"", ""devs"": ""{1}"" }}";

        public static string FakeUserAgentHeader = @"Mozilla/5.0 (Windows NT 6.2; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0";
        public static string ContentTypeAppJsonHeader = @"application/json";
    }
}