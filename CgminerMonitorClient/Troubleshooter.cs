using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using CgminerMonitorClient.PostMakers;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient
{
    public class Troubleshooter
    {
        public void Execute()
        {
            Log.Instance.Info("Starting Troubleshooter...");
            SafelyExecute(PingGoogle);
            SafelyExecute(MakeGet);
            SafelyExecute(MakeWebClientPost);
            if (!PlatformCheck.AreWeRunningUnderWindows())
                SafelyExecute(MakeCurlPost);
            Log.Instance.Info("That's all.");
        }

        private delegate void Method();
        private delegate bool Func(PostMakerType postMakerType);

        private static void SafelyExecute(Method action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Log.Instance.Info("[FAIL] Error:", e);
            }
        }

        private static void MakeCurlPost()
        {
            MakeGenericPost(DoubleTestPost, "curl", PostMakerType.Curl);
        }

        private static void MakeWebClientPost()
        {
            MakeGenericPost(DoubleTestPost, "WebClient", PostMakerType.WebClient);
        }

        private static void MakeGenericPost(Func testPostFunction, string postClientName, PostMakerType postMakerType)
        {
            Log.Instance.InfoFormat("Testing POST request using {0}...", postClientName);
            if (testPostFunction(postMakerType))
                Log.Instance.InfoFormat("[OK] Can send POST requests using {0} to CgminerMonitor cloud service.", postClientName);
            else
                Log.Instance.InfoFormat("[FAIL] Can send POST requests using {0} to CgminerMonitor cloud service.", postClientName);
        }

        private static void MakeGet()
        {
            Log.Instance.Info("Testing GET request...");
            if (TestGetRequest())
                Log.Instance.Info("[OK] CgminerMonitor cloud service is visible.");
            else
                Log.Instance.Info("[FAIL] CgminerMonitor cloud service is not visible.");
        }

        private static void PingGoogle()
        {
            Log.Instance.Info("Pinging google...");
            if (TestPingToGoogle())
                Log.Instance.Info("[OK] Pinging google.");
            else
                Log.Instance.Info("[FAIL] Pinging google.");
        }

        private static bool DoubleTestPost(PostMakerType postMakerType)
        {
            return DoubleTestPostRequestGeneric(MakePostRequest, postMakerType);
        }

        private static bool DoubleTestPostRequestGeneric(Func testPostFunction, PostMakerType postMakerType)
        {
            try
            {
                return testPostFunction(postMakerType); //for unknown reason, first request may fail
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
            return testPostFunction(postMakerType);
        }

        private static bool MakePostRequest(PostMakerType postMakerType)
        {
            using (var client = PostMakerFactory.GetPostMaker(postMakerType, Consts.RequestTimeoutInMiliseconds))
            {
                client.SetContentTypeHeader(Consts.ContentTypeAppJsonHeader);
                client.SetUserAgentHeader(Consts.FakeUserAgentHeader);
                var sentGuid = Guid.NewGuid().ToString("N");
                var result = client.UploadString(new Uri(Consts.TroubleshooterUrl), "\"" + sentGuid + "\"");
                Log.Instance.InfoFormat("Response is: '{0}'", result);
                if (string.IsNullOrEmpty(result))
                    return false;
                return DeobfuscateResponse(result) == sentGuid.Substring(0, 5) + Consts.TroubleshooterResponse;
            }
        }

        private static bool TestGetRequest()
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(Consts.TroubleshooterUrl);
            request.Method = "GET";
            string test;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var ds = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(ds))
                    {
                        test = sr.ReadToEnd();
                    }
                }
            }
            Log.Instance.InfoFormat("Response is: '{0}'", test);
            return DeobfuscateResponse(test) == Consts.TroubleshooterResponse;
        }

        private static string DeobfuscateResponse(string response)
        {
            return response.Trim(new[] { '\"' });
        }

        private static bool TestPingToGoogle()
        {
            var pingSender = new Ping();
            var reply = pingSender.Send("8.8.8.8", 10000);
            if (reply != null && reply.Status == IPStatus.Success)
                return true;
            if (reply == null)
            {
                Log.Instance.Info("Reply from ping is null. Probably this machine doesn't have access to the internet.");
                return false;
            }
            if (reply.Status != IPStatus.Success)
            {
                Log.Instance.InfoFormat("Reply is status is: {0}!", reply.Status);
                Log.Instance.InfoFormat("Address: {0}", reply.Address.ToString());
                Log.Instance.InfoFormat("RoundTrip time: {0}", reply.RoundtripTime);
                Log.Instance.InfoFormat("Time to live: {0}", reply.Options.Ttl);
                Log.Instance.InfoFormat("Don't fragment: {0}", reply.Options.DontFragment);
                Log.Instance.InfoFormat("Buffer size: {0}", reply.Buffer.Length);
                return false;
            }
            return true;
        }
    }
}