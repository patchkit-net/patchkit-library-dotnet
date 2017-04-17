namespace PatchKit.Api
{
    public interface IHttpWebRequestFactory
    {
        IHttpWebRequest Create(string url);
    }
}