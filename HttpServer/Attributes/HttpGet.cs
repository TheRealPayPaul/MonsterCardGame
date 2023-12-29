using Server.Interfaces;

namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGet : Attribute, IHttpMethod
    {
        public readonly string Path;

        public HttpGet(string path = "")
        {
            Path = path;
        }

        public string GetPath()
        {
            return Path;
        }
    }
}
