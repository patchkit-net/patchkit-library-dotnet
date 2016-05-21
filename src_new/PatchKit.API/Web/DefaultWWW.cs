using System;
using System.IO;
using System.Net;
using System.Text;
using JetBrains.Annotations;
using PatchKit.API.Async;

namespace PatchKit.API.Web
{
    /// <summary>
    /// Default implementation for accessing web resources.
    /// </summary>
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

	    public ICancellableAsyncResult BeginDownloadString(string url, CancellableAsyncCallback asyncCallback = null, object state = null)
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
            // Create HTTP web request.
            var httpRequest = WebRequest.Create(url) as HttpWebRequest;

            // Check whether type of request is HTTP.
            // Otherwise throw exception about wrong url which caused request to be different than HTTP.
            if (httpRequest == null)
            {
                throw new FormatException(string.Format("Invaild HTTP url \"{0}\"", url));
            }

            // Register to callback to abort the request when operation is cancelled.
            var cancellationTokenRegistration = cancellationToken.Register(() =>
            {
                httpRequest.Abort();
            });


            // Begin getting response from the url. 
            // Note that this function does initial operations synchronously so it can freeze the operation for some time.
            // TODO: Find better solution so the operation wouldn't be freezed because of that.
            // Source : https://msdn.microsoft.com/pl-pl/library/system.net.httpwebrequest.begingetresponse(v=vs.110).aspx
            var asyncResult = httpRequest.BeginGetResponse(ar =>
            {
            }, null);

            // Cancellation token registration is unregistered when registration is disposed. That's why cancellationTokenRegistration is surrounded by using.
            using (cancellationTokenRegistration)
            {
                // Wait until the requests finishes.
                asyncResult.AsyncWaitHandle.WaitOne((int) _timeout, false);

                // Check whether operation wasn't aborted by the cancellation.
	            cancellationToken.ThrowIfCancellationRequested();

                // Check whether operation wasn't aborted by the timeout.
	            if (!asyncResult.IsCompleted)
	            {
	                throw new TimeoutException();
	            }

                try
                {
                    using (var response = (HttpWebResponse) httpRequest.EndGetResponse(asyncResult))
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                // ReSharper disable once AssignNullToNotNullAttribute
                                var responseEncoding = Encoding.GetEncoding(response.CharacterSet);

                                // Create stream reader for reading the text from the response stream.
                                using (var sr = new StreamReader(responseStream, responseEncoding))
                                {
                                    var responseData = sr.ReadToEnd();

                                    var responseStatusCode = (int) response.StatusCode;

                                    return new WWWResponse<string>(responseData, responseStatusCode);
                                }
                            }
                        }
                    }
                }
                catch (WebException)
                {
                    // WebException can be raised by EndGetResponse when the request has been aborted.
                    // We need to check whether this situation has happend - simply throw cancellation exception if operation has been cancelled.
                    cancellationToken.ThrowIfCancellationRequested();

                    // Otherwise rethrow the exception.
                    throw;
                }

                throw new WebException("Couldn't read response stream.");
	        }
	    }
	}
}

