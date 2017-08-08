using System;
using System.Collections.Generic;
using System.Linq;

namespace PatchKit.Api
{
    /// <summary>
    /// Occurs when there are problems with connection to API.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ApiConnectionException : Exception
    {
        internal readonly List<Exception> MainServerExceptionsList = new List<Exception>();
        
        internal readonly List<Exception> CacheServersExceptionsList = new List<Exception>();
        
        internal ApiConnectionException() : base("Unable to connect to any API servers.")
        {
        }

        /// <summary>
        /// Exceptions that occured during attempts to connect to main server.
        /// </summary>
        public IEnumerable<Exception> MainServerExceptions { get { return MainServerExceptionsList; } }
        
        /// <summary>
        /// Exceptions that occured during attempts to connect to cache servers.
        /// </summary>
        public IEnumerable<Exception> CacheServersExceptions { get { return CacheServersExceptionsList; } }

        public override string ToString()
        {
            var t = base.ToString();

            t += "\n" +
                 "Main server exceptions:\n" +
                 ExceptionsToString(MainServerExceptionsList) +
                 "Cache servers exceptions:\n" +
                 ExceptionsToString(CacheServersExceptionsList);

            return t;
        }

        private string ExceptionsToString(List<Exception> exceptions)
        {
            if (exceptions.Count == 0)
            {
                return "(none)";
            }
            
            var result = string.Empty;
            
            for (var i = 0; i < exceptions.Count; i++)
            {
                result += string.Format("{0}. {1}\n", i, exceptions[i]);
            }

            return result;
        }
    }
}
