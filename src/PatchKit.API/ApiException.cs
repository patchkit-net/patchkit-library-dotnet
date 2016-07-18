using System;

namespace PatchKit.API
{
    /// <summary>
    /// Occurs when there are problems with API.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// API status code.
        /// </summary>
        public readonly int StatusCode;

        internal ApiException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
