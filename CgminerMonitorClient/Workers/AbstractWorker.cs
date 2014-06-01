using System;
using System.Configuration;
using System.Security.Principal;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.PostMakers;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.Workers
{
    public abstract class AbstractWorker : IWorkerDefinition
    {
        private const int SleepSecondsAfterFailure = 2;
        protected readonly string StatisticKey;
        private readonly RetryingHttpClient _client;

        protected AbstractWorker(string statisticsKey)
        {
            StatisticKey = statisticsKey;
            _client = new RetryingHttpClient();
        }

        public void Start(object configObject)
        {
            var config = (Config) configObject;
            try
            {
                Log.Instance.DebugFormat("Starting {0} worker.", StatisticKey);
                if (CheckAvailability())
                    Log.Instance.DebugFormat("'{0}' worker is available.", StatisticKey);
                else
                {
                    Log.Instance.InfoFormat("'{0}' worker is not available.", StatisticKey);
                    return;
                }

                Log.Instance.DebugFormat("Pre-init {0} worker.", StatisticKey);
                PreInit(config);
                Log.Instance.DebugFormat("Pre-init {0} worker succeeded.", StatisticKey);

                while (true)
                {
                    Log.Instance.InfoFormat("Sending {0} data.", StatisticKey);
                    var message = GetInsertMessage(config);
                    Log.Instance.DebugFormat("Getting data for {0} worker.", StatisticKey);
                    message.Stats = GetStats(config);
                    if (string.IsNullOrEmpty(message.Stats))
                    {
                        Log.Instance.InfoFormat("Getting {0} data failed. Sleeping for a {1}s and retrying",
                            StatisticKey, SleepSecondsAfterFailure);
                        Thread.Sleep(SleepSecondsAfterFailure*1000);
                        continue;
                    }

                    Log.Instance.DebugFormat("Getting data for {0} worker succeeded.", StatisticKey);
                    var result = _client.MakePost(message, config.RunOptions.PostMakerType, Consts.StatisticsUrl);

                    if (result.Success)
                    {
                        Log.Instance.InfoFormat("Sending {0} data completed successfully.", StatisticKey);
                        Log.Instance.DebugFormat("Sleeping for {0} seconds.", result.SleepSeconds);
                        Thread.Sleep(TimeSpan.FromSeconds(result.SleepSeconds));
                    }
                    else
                    {
                        Log.Instance.InfoFormat("Error occured while sending {0} data. Error code: {1}, Message: {2}", StatisticKey, result.ErrorCode, result.ErrorMessage);
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                    }
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

        protected abstract bool CheckAvailability();

        protected abstract string GetStats(Config config);

        protected abstract void PreInit(Config config);

        protected virtual StatisticsInsertMessage GetInsertMessage(Config config)
        {
            return new StatisticsInsertMessage(config.WorkerApiKey, StatisticKey);
        }
    }
}