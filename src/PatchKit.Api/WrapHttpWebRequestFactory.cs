using System.Net;

namespace PatchKit.Api
{
    public class WrapHttpWebRequestFactory : IHttpWebRequestFactory
    {
        public IHttpWebRequest Create(string url)
        {
            return new WrapHttpWebRequest((HttpWebRequest) WebRequest.Create(url));
        }
    }
}