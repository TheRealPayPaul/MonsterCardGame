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
            streamWriter.Write($"HTTP/1.1 {(int)resObj.ResponseCode} OK\nServer: Paul\n\n");
        }
    }
}
