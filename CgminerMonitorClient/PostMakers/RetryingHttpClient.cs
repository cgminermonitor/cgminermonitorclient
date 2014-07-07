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
            Log.Instance.DebugFormat("Hitting '{0}' url.", url);
            for (int i = 0; i < Consts.HttpClientRetries; i++)
            {
                Log.Instance.DebugFormat("Making {0}st/nd/th request.", i + 1);
                try
                {
                    using (var client = PostMakerFactory.GetPostMaker(postMakerType, Consts.RequestTimeoutInMiliseconds))
                    {
                        client.SetContentTypeHeader(Consts.ContentTypeAppJsonHeader);
                        client.SetUserAgentHeader(Consts.FakeUserAgentHeader);
                        var result = client.UploadString(new Uri(url),
                            JsonConvert.SerializeObject(message));
                        var deserialized = JsonConvert.DeserializeObject<StatisticsResultMessage>(result);
                        return deserialized;
                    }
                }
                catch (Exception e)
                {
                    Log.Instance.DebugFormat("Error occured (when contacting {0}){1}{2}.", url, Environment.NewLine, e);
                    Log.Instance.InfoFormat("Error occured (when contacting {0}): {1}.", url, e.Message);
                }
                Thread.Sleep(2000);
            }
            return new StatisticsResultMessage {Success = false, ErrorCode = ErrorCodes.ClientError};
        }
    }
}