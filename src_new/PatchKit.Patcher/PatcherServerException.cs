using System;

namespace PatchKit.Patcher
{
    /// <summary>
    /// Occurs when there are problems with server.
    /// </summary>
    public class PatcherServerException : Exception
    {
        public readonly int Status;

        internal PatcherServerException(string message, int status) : base(message)
        {
            Status = status;
        }
    }
}
