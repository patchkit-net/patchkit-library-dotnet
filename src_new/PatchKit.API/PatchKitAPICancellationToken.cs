using System;

namespace PatchKit.API
{
    public abstract class PatchKitAPICancellationToken
    {
        public bool IsCancellationRequested { get; protected set; }

        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }
    }
}
