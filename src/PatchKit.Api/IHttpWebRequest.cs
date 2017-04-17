using System;

namespace PatchKit.Api
{
    public interface IHttpWebRequest
    {
        int Timeout { get; set; }

        Uri Address { get; }

        IHttpWebResponse GetResponse();
    }
}