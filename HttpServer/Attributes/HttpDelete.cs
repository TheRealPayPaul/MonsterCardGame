using Server.Interfaces;

namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDelete : Attribute, IHttpMethod
    {
        public readonly string Path;

        public HttpDelete(string path = "")
        {
            Path = path;
        }

        public string GetPath()
        {
            return Path;
        }
    }
}
