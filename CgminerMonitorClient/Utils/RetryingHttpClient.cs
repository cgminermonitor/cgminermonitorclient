using System;
using System.Net;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using Newtonsoft.Json;

namespace CgminerMonitorClient.Utils
{
    public class RetryingHttpClient
    {
        public StatisticsResultMessage MakePost(object message)
        {
            for (int i = 0; i < Consts.HttpClientRetries; i++)
            {
                Log.Instance.DebugFormat("Making {0}st/nd/th request.", i + 1);
                try
                {
                    using (var client = new NotShittyWebClient())
                    {
                        client.Headers.Add("Content-Type", "application/json");
                        client.Timeout = Consts.RequestTimeoutInMiliseconds;
                        Log.Instance.DebugFormat("About to hit '{0}' url.", Consts.StatisticsUrl);
                        var result = client.UploadString(new Uri(Consts.StatisticsUrl),
                            JsonConvert.SerializeObject(message));
                        var deserialized = JsonConvert.DeserializeObject<StatisticsResultMessage>(result);
                        return deserialized;
                    }
                }
                catch (Exception e)
                {
                    Log.Instance.Debug("Error occured.", e);
                    Log.Instance.Info("Error occured. Retrying.");
                }
                Thread.Sleep(2000);
            }
            return new StatisticsResultMessage {Success = false, ErrorCode = ErrorCodes.ClientError};
        }
    }
}