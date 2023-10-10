using Server;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MonsterCardGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpServer server = new(IPAddress.Loopback, 8080);
            server.SetMaxConnections(50);
            server.MapControllers();
            server.Start();
        }
    }
}