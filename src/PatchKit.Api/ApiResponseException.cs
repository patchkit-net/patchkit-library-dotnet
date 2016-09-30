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
        public int StatusCode { get; private set; }

        internal ApiResponseException(int statusCode) : base(string.Format("Invalid API response - {0}", statusCode))
        {
            StatusCode = statusCode;
        }
    }
}
