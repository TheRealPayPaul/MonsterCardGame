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
                Content = null,
            };
        }

        public static HttpResponseObject Build(object? obj)
        {
            if (obj is HttpResponseObject)
            {
                Console.WriteLine($"INFO [{nameof(HttpResponseFactory)}] trying to build {nameof(HttpResponseObject)} with {nameof(HttpResponseObject)}");
                return (HttpResponseObject)obj;
            }

            ResponseCode responseCode = ResponseCode.Ok;
            object? content = null;

            if (obj is ActionResult result)
            {
                responseCode = result.ResponseCode;
                content = result.Content;
            }
            else if (obj is ResponseCode code)
            {
                responseCode = code;
            }
            else
            {
                content = obj;
            }

            return new()
            {
                ResponseCode = responseCode,
                Server = SERVER_NAME,
                Connection = DEFAULT_CONNECTION,
                ContentType = DEFAULT_CONENT_TYPE,
                Content = content,
            };
        }
    }
}
