namespace PatchKit.API.Web
{
    public struct WWWResponse<T>
    {
        public readonly bool HasBeenCancelled;

        public readonly T Value;

        public readonly int StatusCode;

        public WWWResponse(T value, int statusCode, bool hasBeenCancelled) : this()
        {
            Value = value;
            StatusCode = statusCode;
            HasBeenCancelled = hasBeenCancelled;
        }
    }
}
