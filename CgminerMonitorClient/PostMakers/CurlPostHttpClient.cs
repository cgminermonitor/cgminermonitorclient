using System;
using System.Diagnostics;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.PostMakers
{
    public class CurlPostHttpClient : IPostMaker
    {
        private const string RequestTemplate = @"-X POST -d @- {0} --header ""Content-Type:{1}"" -A ""{2}"" -m {3} -sS";

        //time in milliseconds
        private readonly int _timeout;
        private string _contentTypeHeader = Consts.ContentTypeAppJsonHeader;
        private string _userAgentHeader;

        public CurlPostHttpClient(int timeout)
        {
            _timeout = timeout;
        }

        public string UploadString(Uri address, string data)
        {
            if (string.IsNullOrEmpty(_userAgentHeader))
                throw new Exception("User-Agent header was not set.");

            var parameters = string.Format(RequestTemplate, address, _contentTypeHeader, _userAgentHeader, _timeout / 1000);
            var ps = new ProcessStartInfo("curl", parameters)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };
            Log.Instance.DebugFormat("Starting curl with parameters '{0}'", parameters);

            using (var p = Process.Start(ps))
            {
                if (p == null)
                    throw new Exception("Could not start curl process.");
                p.StandardInput.WriteLine(data);
                p.StandardInput.Flush();
                p.StandardInput.Close();
                var output = p.StandardOutput.ReadToEnd();

                // waits for the process to exit
                // Must come *after* StandardOutput is "empty"
                // so that we don't deadlock because the intermediate
                // kernel pipe is full.
                p.WaitForExit();
                if (p.ExitCode > 0)
                    throw new Exception(
                        string.Format("Could not POST using curl. Curl error code: '{0}'. Curl returned: '{1}'",
                            p.ExitCode, output));
                return output;
            }
        }

        public void Dispose()
        {
        }

        public void SetContentTypeHeader(string type)
        {
            _contentTypeHeader = type;
        }

        public void SetUserAgentHeader(string userAgent)
        {
            _userAgentHeader = userAgent;
        }
    }
}