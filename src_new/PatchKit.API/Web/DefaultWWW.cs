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

	    public WWWResponse<string> DownloadString (string url, PatchKitAPICancellationToken cancellationToken)
		{
			var httpRequest = WebRequest.Create (url) as HttpWebRequest;

			if (httpRequest == null) 
			{
				throw new FormatException (string.Format("Provided url {0} is not vaild HTTP url.", url)));
			}

	        string responseData = string.Empty;
	        int responseStatusCode = 0;
	        Exception responseException = null;

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
                        responseData = sr.ReadToEnd();

                        responseStatusCode = (int) response.StatusCode;
		            }
		        }
		        catch(Exception exception)
		        {
		            responseException = exception;
		        }
		    }, null);
			
            asyncResult.AsyncWaitHandle.WaitOne()
            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            

			while (!asyncResult.IsCompleted) 
			{
				if (cancellationToken.IsCancellationRequested || watch.ElapsedMilliseconds > _timeout) 
				{
					httpRequest.Abort ();
					break;
				}
				System.Threading.Thread.Sleep (1);
			}

            return new 
		}
	}
}

