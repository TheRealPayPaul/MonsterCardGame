using Server.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class HttpResponseObject
    {
        public ResponseCode ResponseCode { get; set; } = ResponseCode.Ok;
        public string Server { get; set; } = string.Empty;
        public DateTime Date {  get; set; }
        public string Connection { get; set; } = string.Empty;
        public int ContentLength { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
