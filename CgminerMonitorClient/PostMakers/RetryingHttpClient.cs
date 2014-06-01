using System;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Utils;
using Newtonsoft.Json;

namespace CgminerMonitorClient.PostMakers
{
    public class RetryingHttpClient
    {
        public StatisticsResultMessage MakePost(object message, PostMakerType postMakerType, string url)
        {
            for (int i = 0; i < Consts.HttpClientRetries; i++)
            {
                Log.Instance.DebugFormat("Making {0}st/nd/th request.", i + 1);
                try
                {
                    using (var client = PostMakerFactory.GetPostMaker(postMakerType, Consts.RequestTimeoutInMiliseconds))
                    {
                        client.SetContentTypeHeader("application/json");
                        Log.Instance.DebugFormat("About to hit '{0}' url.", url);
                        var result = client.UploadString(new Uri(url),
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