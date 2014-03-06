using System;
using System.Net;

namespace CgminerMonitorClient.Utils
{
    public class NotShittyWebClient : WebClient
    {
        //time in milliseconds
        private int _timeout;

        public NotShittyWebClient()
        {
            _timeout = 60000;
        }

        public NotShittyWebClient(int timeout)
        {
            _timeout = timeout;
        }

        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest result = base.GetWebRequest(address);
            if (result != null)
                result.Timeout = _timeout;
            return result;
        }
    }
}