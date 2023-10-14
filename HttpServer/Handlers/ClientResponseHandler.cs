using Server.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handlers
{
    internal static class ClientResponseHandler
    {
        public static void Handle(StreamWriter streamWriter, HttpResponseObject resObj)
        {
            Console.WriteLine($"[{(int)resObj.ResponseCode}]");

            streamWriter.WriteLine($"HTTP/1.1 {(int)resObj.ResponseCode} {ResponseCodeConverter.EnumToString(resObj.ResponseCode)}");
            streamWriter.WriteLine($"Server: {resObj.Server}");
            streamWriter.WriteLine($"Date: {DateTime.Now}");
            streamWriter.WriteLine($"Connection: {resObj.Connection}");
            streamWriter.WriteLine($"Content-Length: {resObj.ContentLength}");
            streamWriter.WriteLine($"Content-Type: {resObj.ContentType}");
            streamWriter.WriteLine();
            if (resObj.ContentLength > 0)
            {
                streamWriter.WriteLine(resObj.Content);
            }
        }
    }
}
