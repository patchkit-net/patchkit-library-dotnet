using System;

namespace PatchKit.Api
{
    /// <summary>
    /// Occurs when there are problems with API response.
    /// </summary>
    /// <seealso cref="System.Exception"/>
    public class ApiException : Exception
    {
        internal ApiException(string reason) : base(reason)
        {
        }
    }
}