using System;

namespace PatchKit.Patcher
{
    /// <summary>
    /// Occurs when there are problems with server.
    /// </summary>
    public class PatchKitPatcherServerException : Exception
    {
        public readonly int Status;

        internal PatchKitPatcherServerException(string message, int status) : base(message)
        {
            Status = status;
        }
    }
}
