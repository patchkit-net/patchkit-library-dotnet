using System;
using System.Net;

namespace PatchKit.Api
{
    public class WrapHttpWebRequest : IHttpWebRequest
    {
        private readonly HttpWebRequest _httpWebRequest;

        public int Timeout
        {
            get { return _httpWebRequest.Timeout; }
            set { _httpWebRequest.Timeout = value; }
        }

        public Uri Address { get { return _httpWebRequest.Address; } }

        public WrapHttpWebRequest(HttpWebRequest httpWebRequest)
        {
            _httpWebRequest = httpWebRequest;
        }

        public IHttpWebResponse GetResponse()
        {
            return new WrapHttpWebResponse(GetHttpResponse());
        }

        private HttpWebResponse GetHttpResponse()
        {
            try
            {
                return (HttpWebResponse) _httpWebRequest.GetResponse();
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.ProtocolError && webException.Response != null)
                {
                    return (HttpWebResponse) webException.Response;
                }
                throw;
            }
        }
    }
}