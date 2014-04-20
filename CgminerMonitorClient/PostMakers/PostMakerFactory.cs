using System;

namespace CgminerMonitorClient.PostMakers
{
    public class PostMakerFactory
    {
        public static IPostMaker GetPostMaker(PostMakerType postMakerType, int timeout)
        {
            switch (postMakerType)
            {
                case PostMakerType.WebClient:
                    return new NotShittyWebClient(timeout);
                case PostMakerType.Curl:
                    return new CurlPostHttpClient(timeout);
                default:
                    throw new ArgumentOutOfRangeException("postMakerType");
            }
        }
    }
}