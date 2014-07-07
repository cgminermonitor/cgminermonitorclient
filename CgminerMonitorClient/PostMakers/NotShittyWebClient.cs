﻿using System;
using System.Net;

namespace CgminerMonitorClient.PostMakers
{
    public class NotShittyWebClient : WebClient, IPostMaker
    {
        //time in milliseconds
        private int _timeout;

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

        public void SetContentTypeHeader(string type)
        {
            Headers.Add("Content-Type", type);
        }

        public void SetUserAgentHeader(string userAgent)
        {
            Headers.Add("User-Agent", userAgent);
        }
    }
}