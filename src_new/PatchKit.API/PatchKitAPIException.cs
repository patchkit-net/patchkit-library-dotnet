using System;

namespace PatchKit.API
{
    /// <summary>
    /// Occurs when there are problems with API.
    /// </summary>
    public class PatchKitAPIException : Exception
    {
        public readonly int Status;

        internal PatchKitAPIException(string message, int status) : base(message)
        {
            Status = status;
        }
    }
}
