namespace Rewards.Middleware
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(int statusCode, string message) =>
            (StatusCode, Message) = (statusCode, message);

        public int StatusCode { get; }

        public string Message { get; }
    }
}
