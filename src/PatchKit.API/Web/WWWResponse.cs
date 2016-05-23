namespace PatchKit.API.Web
{
    public struct WWWResponse<T>
    {
        public readonly T Value;

        public readonly int StatusCode;

        public WWWResponse(T value, int statusCode) : this()
        {
            Value = value;
            StatusCode = statusCode;
        }
    }
}
