namespace PatchKit.API.Web
{
    public interface IWWW
    {
        WWWResponse<string> DownloadString(string url, PatchKitAPICancellationToken cancellationToken);
    }
}