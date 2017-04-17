using System.IO;
using System.Net;

namespace PatchKit.Api
{
    public class WrapHttpWebResponse : IHttpWebResponse
    {
        private readonly HttpWebResponse _httpWebResponse;

        public string CharacterSet { get { return _httpWebResponse.CharacterSet; } }

        public HttpStatusCode StatusCode { get { return _httpWebResponse.StatusCode; } }

        public WrapHttpWebResponse(HttpWebResponse httpWebResponse)
        {
            _httpWebResponse = httpWebResponse;
        }

        public Stream GetResponseStream()
        {
            return _httpWebResponse.GetResponseStream();
        }
    }
}