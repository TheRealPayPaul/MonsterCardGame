using Server.Converter;
using System.Text.Json;

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

            try
            {
                streamWriter.Write(response);
            }
            catch (IOException e)
            {
                Console.WriteLine($"Client closed connection while server processed its request. Exception: {e.Message}");
            }
        }
    }
}
