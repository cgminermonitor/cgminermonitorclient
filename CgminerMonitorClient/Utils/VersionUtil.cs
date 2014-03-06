using System.Reflection;

namespace CgminerMonitorClient.Utils
{
    public static class VersionUtil
    {
        public static string GetAsString()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}