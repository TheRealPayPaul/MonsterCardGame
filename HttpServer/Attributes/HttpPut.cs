using Server.Interfaces;

namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPut : Attribute, IHttpMethod
    {
        public readonly string Path;

        public HttpPut(string path = "")
        {
            Path = path;
        }

        public string GetPath()
        {
            return Path;
        }
    }
}
