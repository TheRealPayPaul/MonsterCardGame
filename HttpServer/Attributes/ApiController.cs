namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiController : Attribute
    {
        public readonly string Path;

        public ApiController(string path)
        {
            Path = path;
        }
    }
}
