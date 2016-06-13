namespace PatchKit.API.Web
{
    public struct StringDownloadResult
    {
        /// <summary>
        /// String value.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// HTTP status code.
        /// </summary>
        public readonly int StatusCode;

        public StringDownloadResult(string value, int statusCode) : this()
        {
            Value = value;
            StatusCode = statusCode;
        }
    }
}
