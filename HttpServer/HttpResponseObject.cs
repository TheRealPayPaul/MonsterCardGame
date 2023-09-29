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
    }
}
