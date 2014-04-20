using System;

namespace CgminerMonitorClient.PostMakers
{
    public interface IPostMaker: IDisposable
    {
        void SetContentTypeHeader(string type);
        string UploadString(Uri address, string data);
    }
}