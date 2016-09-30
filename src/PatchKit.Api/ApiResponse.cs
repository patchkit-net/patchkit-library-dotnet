using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PatchKit.Api
{
    internal class ApiResponse : IApiResponse
    {
        public ApiResponse(HttpWebResponse httpWebResponse)
        {
            HttpWebResponse = httpWebResponse;

            var responseStream = HttpWebResponse.GetResponseStream();

            if (HttpWebResponse.CharacterSet == null || responseStream == null)
            {
                throw new WebException("Invalid response from API server.");
            }

            var responseEncoding = Encoding.GetEncoding(HttpWebResponse.CharacterSet);

            using (var streamReader = new StreamReader(responseStream, responseEncoding))
            {
                Body = streamReader.ReadToEnd();
            }
        }

        public HttpWebResponse HttpWebResponse { get; private set; }

        public string Body { get; private set; }

        public JToken GetJson()
        {
            return JToken.Parse(Body);
        }

        void IDisposable.Dispose()
        {
            ((IDisposable) HttpWebResponse).Dispose();
        }
    }
}
