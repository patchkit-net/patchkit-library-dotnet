using PatchKit.API.Async;

namespace PatchKit.API.Web
{
    public interface IWWW
    {
        ICancellableAsyncResult BeginDownloadString(string url, CancellableAsyncCallback asyncCallback = null, object state = null);

        WWWResponse<string> EndDownloadString(ICancellableAsyncResult asyncResult);
    }
}