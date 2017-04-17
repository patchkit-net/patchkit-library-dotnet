using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace PatchKit.Api
{
    /// <summary>
    /// API response.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IApiResponse : IDisposable
    {
        /// <summary>
        /// HTTP web response.
        /// </summary>
        IHttpWebResponse HttpWebResponse { get; }

        /// <summary>
        /// Response body.
        /// </summary>
        string Body { get; }

        /// <summary>
        /// Returns body parsed to JSON.
        /// </summary>
        JToken GetJson();
    }
}
