namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ApplyMiddleware : Attribute
    {
        public readonly IEnumerable<string> MiddlewareNames;

        public ApplyMiddleware(string firstMiddlewareName, params string[] middlewareNames)
        {
            MiddlewareNames = new[] { firstMiddlewareName }.Concat(middlewareNames);
        }
    }
}
