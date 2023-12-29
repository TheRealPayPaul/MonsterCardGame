using Server.Interfaces;

namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPost : Attribute, IHttpMethod
    {
        public readonly string Path;

        public HttpPost(string path = "")
        {
            Path = path;
        }

        public string GetPath()
        {
            return Path;
        }
    }
}
