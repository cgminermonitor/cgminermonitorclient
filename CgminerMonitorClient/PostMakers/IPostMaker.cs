using System;

namespace CgminerMonitorClient.PostMakers
{
    public interface IPostMaker: IDisposable
    {
        void SetContentTypeHeader(string type);
        void SetUserAgentHeader(string userAgent);
        string UploadString(Uri address, string data);
    }
}