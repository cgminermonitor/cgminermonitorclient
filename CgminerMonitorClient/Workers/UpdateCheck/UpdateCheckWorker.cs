using System;
using System.IO;
using System.Net;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.Workers.UpdateCheck
{
    public class UpdateCheckWorker : IWorkerDefinition
    {
        public void Start(object configObject)
        {
            var config = (Config)configObject;
            try
            {
                Log.Instance.Debug("Starting update check worker.");

                Log.Instance.DebugFormat("Sleeping {0}s before first update check.", Consts.FirstUpdateCheckSleepTime.TotalSeconds);
                Thread.Sleep(Consts.FirstUpdateCheckSleepTime);
                Log.Instance.Debug("First updates check.");
                CheckForUpdates();
                Log.Instance.Debug("First updates check completed.");
                while (true)
                {
                    Log.Instance.DebugFormat("Sleeping {0}h before updates check.", Consts.NormalUpdateCheckSleepTime.TotalHours);
                    Thread.Sleep(Consts.NormalUpdateCheckSleepTime);
                    Log.Instance.Info("Updates check.");
                    CheckForUpdates();
                }
            }
            catch (ThreadAbortException)
            {
                //its ok.
            }
            catch (Exception e)
            {
                Log.Instance.Info("Unknown error occured.", e);
            }
        }

        private static void CheckForUpdates()
        {
            var currentClientMetadata = ClientMetadata.GetCurrentClientMetadata();
            var url = Consts.GetUpdateCheckUrl(currentClientMetadata);
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            string latestVersion;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var ds = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(ds))
                    {
                        latestVersion = sr.ReadToEnd();
                    }
                }
            }
            if (!string.IsNullOrEmpty(latestVersion) && !string.Equals(latestVersion, currentClientMetadata.ClientVersion, StringComparison.Ordinal))
                Log.Instance.InfoFormat("You are running {0} version, whereas {1} is the latest. Please consider updating the client.", currentClientMetadata.ClientVersion, latestVersion);
        }
    }
}