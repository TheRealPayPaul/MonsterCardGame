using Server.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Factories
{
    public static class HttpResponseFactory
    {
        private const string SERVER_NAME = "Panosch Http Server";
        private const string DEFAULT_CONNECTION = "Closed";
        private const string DEFAULT_CONENT_TYPE = "application/json";

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

        public static HttpResponseObject Build(object? obj)
        {
            ResponseCode responseCode = ResponseCode.Ok;
            string content = string.Empty;

            if (obj is ActionResult)
            {
                ActionResult result = (ActionResult)obj;
                responseCode = result.ResponseCode;
                obj = result.Content;
            }

            if (obj != null)
                content = JsonSerializer.Serialize(obj);

            return new()
            {
                ResponseCode = responseCode,
                Server = SERVER_NAME,
                Connection = DEFAULT_CONNECTION,
                ContentType = DEFAULT_CONENT_TYPE,
                ContentLength = content.Length,
                Content = content,
            };
        }
    }
}
