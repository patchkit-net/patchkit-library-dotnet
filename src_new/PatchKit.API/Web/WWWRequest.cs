using System;

namespace PatchKit.API.Web
{
    public class WWWRequest<T>
    {
        public readonly string Url;

        public bool IsCancelled { get; internal set; }

        internal bool IsCompleted { get; private set; }

        internal T Value { get; private set; }

        internal int StatusCode { get; private set; }

        internal Exception Error { get; private set; }

        internal WWWRequest(string url)
        {
            IsCompleted = false;
            Url = url;
        }

        public void CompleteWithError(Exception error)
        {
            Error = error;
        }

        public void Complete(T value, int statusCode)
        {
            IsCompleted = true;
            StatusCode = statusCode;
            Value = value;
        }
    }
}
