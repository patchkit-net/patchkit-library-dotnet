using System.IO;
using System.Net;

namespace PatchKit.Api
{
    public interface IHttpWebResponse
    {
        Stream GetResponseStream();

        string CharacterSet { get; }

        HttpStatusCode StatusCode { get; }
    }
}