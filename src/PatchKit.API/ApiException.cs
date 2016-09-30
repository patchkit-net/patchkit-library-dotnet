using System;

namespace PatchKit.Api
{
    /// <summary>
    /// Occurs when there are problems with API.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// API status code.
        /// </summary>
        public int StatusCode { get; private set; }

        internal ApiException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
