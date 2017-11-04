using System;

namespace PatchKit.Api
{
    /// <summary>
    /// Occurs when there are problems with API response.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ApiResponseException : Exception
    {
        /// <summary>
        /// API status code.
        /// </summary>
        public int StatusCode { get; }

        internal ApiResponseException(int statusCode) : base($"API server returned status code {statusCode}")
        {
            StatusCode = statusCode;
        }
    }
}
