using Server.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class HttpRequestObject
    {
        public RequestMethod RequestType { get; set; }
        public string Path { get; set; } = string.Empty;
        public string ContentString { get; set; } = string.Empty;
        public string RawBody { get; set; } = string.Empty;
        public Dictionary<string, string> RequestParameters { get; } = new();
        public Dictionary<string, string> Headers { get; } = new();
        public Dictionary<string, string> DynamicPathParameters { get; } = new();
    }
}
