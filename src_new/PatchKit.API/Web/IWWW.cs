namespace PatchKit.API.Web
{
    public interface IWWW
    {
        void DownloadString(WWWRequest<string> request);
    }
}
