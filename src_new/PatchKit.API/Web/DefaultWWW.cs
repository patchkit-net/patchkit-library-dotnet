using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace PatchKit.API.Web
{
	public class DefaultWWW : IWWW
	{
	    private readonly long _timeout;

	    public DefaultWWW(long timeout)
	    {
	        _timeout = timeout;
	    }

	    public DefaultWWW() : this(15000)
        {
	    }

	    public void DownloadString (WWWRequest<string> request)
		{
			var httpRequest = WebRequest.Create (request.Url) as HttpWebRequest;

			if (httpRequest == null) 
			{
				request.CompleteWithError (new FormatException (string.Format("Invaild URL {0}", request.Url)));
				return;
			}

		    var asyncResult = httpRequest.BeginGetResponse(ar => 
		    {
		        try
		        {
		            var response = (HttpWebResponse)httpRequest.EndGetResponse(ar);

		            // ReSharper disable once AssignNullToNotNullAttribute
		            var responseEncoding = Encoding.GetEncoding(response.CharacterSet);

		            // ReSharper disable once AssignNullToNotNullAttribute
		            using (var sr = new StreamReader(response.GetResponseStream(), responseEncoding))
		            {
		                string data = sr.ReadToEnd();

		                request.Complete(data, (int)response.StatusCode);
		            }
		        }
		        catch(Exception exception)
		        {
		            request.CompleteWithError(exception);
		        }
		    }, null);
			
            Stopwatch watch = new Stopwatch();
            watch.Start();

			while (!asyncResult.IsCompleted) 
			{
				if (request.IsCancelled || watch.ElapsedMilliseconds > _timeout) 
				{
					httpRequest.Abort ();
					break;
				}
				System.Threading.Thread.Sleep (1);
			}
		}
	}
}

