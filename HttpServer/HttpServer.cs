﻿using Server.Enums;
using Server.Handlers;
using Server.Utility;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

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
            TcpListener server = new(new IPEndPoint(_ipAdress, _port));
            server.Start();

            Console.WriteLine($"Listening on port {_port}...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                Thread thread = new(() => ClientSocketHandler.Handle(client, _requestTree));
                thread.Start();
            }
        }
    }
}
