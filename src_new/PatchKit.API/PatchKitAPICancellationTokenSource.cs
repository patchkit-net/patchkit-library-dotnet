namespace PatchKit.API
{
    internal class PatchKitAPICancellationTokenSource : PatchKitAPICancellationToken
    {
        public void Cancel()
        {
            IsCancellationRequested = true;
        }
    }
}
