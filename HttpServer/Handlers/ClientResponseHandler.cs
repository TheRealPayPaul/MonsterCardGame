using Server.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Handlers
{
    internal static class ClientResponseHandler
    {
        public static void Handle(StreamWriter streamWriter, HttpResponseObject resObj)
        {
            Console.WriteLine($"[{(int)resObj.ResponseCode}]");

            string response = string.Empty;
            response += $"HTTP/1.1 {(int)resObj.ResponseCode} {ResponseCodeConverter.EnumToString(resObj.ResponseCode)}\n";
            response += $"Server: {resObj.Server}\n";
            response += $"Date: {DateTime.Now}\n";
            response += $"Connection: {resObj.Connection}\n";
            if (resObj.Content != null)
            {
                string content = JsonSerializer.Serialize(resObj.Content);
                response += $"Content-Length: {content.Length}\n";
                response += $"Content-Type: {resObj.ContentType}\n\n";
                response += $"{content}\n";
            }
            else
            {
                response += "\n";
            }

            streamWriter.Write(response);
        }
    }
}
