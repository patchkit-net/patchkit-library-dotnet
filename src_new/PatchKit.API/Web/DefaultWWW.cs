using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using PatchKit.API.Async;

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

	    public ICancellableAsyncResult BeginDownloadString(string url, CancellableAsyncCallback asyncCallback, object state)
	    {
	        return new AsyncResult<WWWResponse<string>>(cancellationToken => DownloadString(url, cancellationToken), asyncCallback, state);
	    }

	    public WWWResponse<string> EndDownloadString([NotNull] ICancellableAsyncResult asyncResult)
	    {
	        var result = asyncResult as AsyncResult<WWWResponse<string>>;

            if (result == null)
            {
                throw new ArgumentException("asyncResult");
            }

	        return result.FetchResultsFromAsyncOperation();
	    }

	    private WWWResponse<string> DownloadString(string url, AsyncCancellationToken cancellationToken)
	    {
            var httpRequest = WebRequest.Create(url) as HttpWebRequest;

            if (httpRequest == null)
            {
                throw new FormatException(string.Format("Provided url {0} is not vaild HTTP url.", url));
            }

            string responseData = string.Empty;
            int responseStatusCode = 0;
            Exception responseException = null;

	        ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            var asyncResult = httpRequest.BeginGetResponse(ar =>
            {
                try
                {
                    var response = (HttpWebResponse) httpRequest.EndGetResponse(ar);

                    // ReSharper disable once AssignNullToNotNullAttribute
                    var responseEncoding = Encoding.GetEncoding(response.CharacterSet);

                    // ReSharper disable once AssignNullToNotNullAttribute
                    using (var sr = new StreamReader(response.GetResponseStream(), responseEncoding))
                    {
                        responseData = sr.ReadToEnd();

                        responseStatusCode = (int) response.StatusCode;
                    }
                }
                catch (Exception exception)
                {
                    responseException = exception;
                }
                finally
                {
                    manualResetEvent.Set();
                }
            }, null);

            Timer timoutTimer = new Timer(state =>
            {
                httpRequest.Abort();
            });
	        timoutTimer.Change(_timeout, Timeout.Infinite);

            cancellationToken.Register(() =>
            {
                httpRequest.Abort();
            });

            manualResetEvent.WaitOne();

            cancellationToken.ThrowIfCancellationRequested();

	        if (responseException != null)
	        {
	            throw responseException;
	        }

            return new WWWResponse<string>(responseData, responseStatusCode);
	    }
	}
}

