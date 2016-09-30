using System;

namespace PatchKit.Api
{
    /// <summary>
    /// Occurs when there are problems with connection to API.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ApiConnectionException : Exception
    {
        internal ApiConnectionException() : base("Unable to connect to API.")
        {
        }
    }
}
