using System;
using System.IO;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Utils;
using NAppUpdate.Framework;
using NAppUpdate.Framework.Sources;

namespace CgminerMonitorClient.Workers.Update
{
    public class UpdateWorker : IWorkerDefinition
    {
        public void Start(object configObject)
        {
            var config = (Config) configObject;
            try
            {
                Log.Instance.Debug("Starting updates worker.");

                Log.Instance.Debug("Pre-init updates worker.");
                var updateManager = PreInit(config);
                Log.Instance.Debug("Pre-init updates worker succeeded.");

                Log.Instance.DebugFormat("Sleeping {0}s before first update check.", Consts.FirstUpdateCheckSleepTime.TotalSeconds);
                Thread.Sleep(Consts.FirstUpdateCheckSleepTime);
                Log.Instance.Debug("First updates check.");
                CheckForUpdates(updateManager);
                Log.Instance.Debug("First updates check completed.");
                while (true)
                {
                    Log.Instance.DebugFormat("Sleeping {0}h before updates check.", Consts.NormalUpdateCheckSleepTime.TotalHours);
                    Thread.Sleep(Consts.NormalUpdateCheckSleepTime);
                    Log.Instance.Info("Updates check.");
                    CheckForUpdates(updateManager);
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

        private static UpdateManager PreInit(Config config)
        {
            var updManager = UpdateManager.Instance;
            var url = Consts.GetUpdateUrl(ClientMetadata.GetCurrentClientMetadata());
            Log.Instance.DebugFormat("Update url: '{0}'.", url);
            updManager.UpdateSource = new SimpleWebSource(url);
            updManager.Config.TempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CgminerMonitorUpdates");
            updManager.ReinstateIfRestarted();

            return updManager;
        }

        private static void CheckForUpdates(UpdateManager updateManager)
        {
            try
            {
                if (UpdateManager.Instance.State == UpdateManager.UpdateProcessState.Checked ||
                    UpdateManager.Instance.State == UpdateManager.UpdateProcessState.AfterRestart ||
                    UpdateManager.Instance.State == UpdateManager.UpdateProcessState.AppliedSuccessfully)
                    UpdateManager.Instance.CleanUp();

                // Throws exceptions in case of bad arguments or unexpected results
                updateManager.CheckForUpdates();
            }
            catch (NAppUpdateException ex)
            {
                // This indicates a feed or network error; ex will contain all the info necessary
                // to deal with that
                Log.Instance.Debug("NAppUpdateException error: ", ex);
                return;
            }
            catch (Exception ex)
            {
                Log.Instance.Info("Error while trying to get updates.");
                Log.Instance.Debug("Exception: ", ex);
                return;
            }


            if (updateManager.UpdatesAvailable == 0)
            {
                Log.Instance.Info("Client is up to date");
                return;
            }

            Log.Instance.InfoFormat("Updates are available to client ({0} total). Downloading and preparing...", updateManager.UpdatesAvailable);


            try
            {
                updateManager.PrepareUpdates();
            }
            catch (Exception ex)
            {
                Log.Instance.Info("Updates preperation failed.");
                Log.Instance.Debug("Exception:", ex);
                return;
            }

            OnPrepareUpdatesCompleted(updateManager);
        }

        private static void OnPrepareUpdatesCompleted(UpdateManager updateManager)
        {
            Log.Instance.Info("Updates are ready to install. Installing.");

            // This is a synchronous method by design, make sure to save all user work before calling
            // it as it might restart your application
            try
            {
                updateManager.ApplyUpdates(true, true, true);
            }
            catch (Exception ex)
            {
                Log.Instance.Info("Error while trying to install software updates.");
                Log.Instance.Debug("Exception:", ex);
            }

            if (updateManager.State == UpdateManager.UpdateProcessState.RollbackRequired)
                updateManager.RollbackUpdates();
        }
    }
}