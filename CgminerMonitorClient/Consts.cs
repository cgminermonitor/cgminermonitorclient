using System;
using CgminerMonitorClient.CgminerMonitor.Common;

namespace CgminerMonitorClient
{
    public static class Consts
    {
        public static string DefaultConfigFileName = "CgminerMonitorClient.config";

#if FORDEBUGGING
        public static string StatisticsUrl = "http://127.0.0.1:81/Statistics/";
        public static int RequestTimeoutInMiliseconds = (int)TimeSpan.FromDays(1).TotalMilliseconds;
        public static string ClientUpdateTemplateUrl = "http://127.0.0.1:81/ClientUpdates/{0}/{1}/{2}";
        public static TimeSpan FirstUpdateCheckSleepTime = TimeSpan.FromSeconds(3);
        public static TimeSpan NormalUpdateCheckSleepTime = TimeSpan.FromMinutes(3);
#endif
#if FORRELEASE
        public static string StatisticsUrl = "http://api.cgminermonitor.com/Statistics/";
        public static int RequestTimeoutInMiliseconds = (int)TimeSpan.FromSeconds(3).TotalMilliseconds;
        public static string ClientUpdateTemplateUrl = "http://api.cgminermonitor.com/ClientUpdates/{0}/{1}/{2}";
        public static TimeSpan FirstUpdateCheckSleepTime = TimeSpan.FromSeconds(30);
        public static TimeSpan NormalUpdateCheckSleepTime = TimeSpan.FromHours(3);
#endif

        public static string GetUpdateUrl(ClientMetadata clientMetadata)
        {
            return string.Format(ClientUpdateTemplateUrl,
                (int) clientMetadata.ClientPlatform, (int) clientMetadata.ClientArchitecture, clientMetadata.ClientVersion);
        }

        public static int HttpClientRetries = 5;
    }
}