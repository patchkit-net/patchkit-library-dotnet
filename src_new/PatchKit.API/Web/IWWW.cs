using PatchKit.API.Async;

namespace PatchKit.API.Web
{
    public interface IWWW
    {
        ICancellableAsyncResult BeginDownloadString(string url, CancellableAsyncCallback asyncCallback, object state);

        WWWResponse<string> EndDownloadString(ICancellableAsyncResult asyncResult);
    }
}