using Server.Enums;

namespace Server
{
    public class HttpRequestObject
    {
        public RequestMethod RequestType { get; set; }
        public string Path { get; set; } = string.Empty;
        public string ContentString { get; set; } = string.Empty;
        public string RawBody { get; set; } = string.Empty;
        public object? SessionContent { get; set; }
        public Dictionary<string, string> RequestParameters { get; } = new();
        public Dictionary<string, string> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> DynamicPathParameters { get; } = new();
    }
}
