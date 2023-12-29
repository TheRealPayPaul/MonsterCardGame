namespace Server.Interfaces
{
    public interface IMiddleware
    {
        public object? Invoke(HttpRequestObject reqObj);
    }
}
