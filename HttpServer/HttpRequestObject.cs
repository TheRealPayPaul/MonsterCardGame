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
        public int? ContentLength { get { return GetContentLength(); } }
        public char[] Body { get; set; } = new char[0];
        public List<KeyValuePair<string, string>> RequestParameters { get; } = new();
        public List<KeyValuePair<string, string>> Headers { get; } = new();
        public List<KeyValuePair<string, string>> DynamicPathParameters { get; } = new();

        public override string ToString()
        {
            return $"{RequestType} {Path} {RequestParameters} {Headers.Count}";
        }

        private int? GetContentLength()
        {
            KeyValuePair<string, string> pair = Headers.Find(pair => pair.Key == "Content-Length");

            if (pair.Key == null)
                return null;

            if (!int.TryParse(pair.Value, out int value))
                return null;

            return value;
        }
    }
}
