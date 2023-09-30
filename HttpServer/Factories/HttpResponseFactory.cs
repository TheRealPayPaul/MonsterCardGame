using Server.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Factories
{
    public static class HttpResponseFactory
    {
        private const string SERVER_NAME = "Panosch Http Server";
        private const string DEFAULT_CONNECTION = "Closed";

        public static HttpResponseObject Build(ResponseCode responseCode)
        {
            return new()
            {
                ResponseCode = responseCode,
                Server = SERVER_NAME,
                Connection = DEFAULT_CONNECTION,
                ContentLength = 0,
                Content = string.Empty,
            };
        }
    }
}
