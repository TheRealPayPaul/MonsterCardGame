using Server.Attributes;
using Server.Enums;
using Server.Handlers;
using Server.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class HttpServer
    {
        private IPAddress _ipAdress;
        private int _port;
        private int _maxConnections = 10;
        private RequestTree _requestTree = new();

        public HttpServer(IPAddress ipAddress, int port)
        {
            _ipAdress = ipAddress;
            _port = port;
        }

        public void SetMaxConnections(int maxConnections)
        {
            _maxConnections = maxConnections;
        }

        public void MapControllers()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            List<(string, RequestMethod, EndpointChain)> endpointChains = RouteCollector.CrawlAssembly(assembly);
            foreach ((string path, RequestMethod requestMethod, EndpointChain endpointChain) in endpointChains)
                _requestTree.AddRoute(path, requestMethod, endpointChain);
        }

        public void Start()
        {
            Socket serverSock = new(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            serverSock.Bind(new IPEndPoint(_ipAdress, _port));
            serverSock.Listen(_maxConnections);

            while (true)
            {
                Socket clientSock = serverSock.Accept();

                Thread thread = new(() => ClientSocketHandler.Handle(clientSock, _requestTree));
                thread.Start();
            }
        }
    }
}
