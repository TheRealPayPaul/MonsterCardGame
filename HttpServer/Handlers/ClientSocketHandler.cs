using Server.Enums;
using Server.Exceptions;
using Server.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handlers
{
    internal static class ClientSocketHandler
    {
        public static void Handle(Socket clientSocket, RequestTree requestTree)
        {
            using (NetworkStream networkStream = new(clientSocket))
            using (StreamWriter streamWriter = new(networkStream))
            {
                HttpResponseObject resObj;
                try
                {
                    HttpRequestObject reqObj = ClientRequestHandler.Handle(networkStream);
                    
                    EndpointChain? endpointChain = requestTree.GetEndpoint(reqObj);
                    if (endpointChain == null)
                        throw new NotFoundException($"[ClientSocketHandler] EndpointChain Not Found: {reqObj.RequestType} {reqObj.Path}");

                    resObj = endpointChain.Invoke(reqObj);
                }
                catch (BadRequestException ex)
                {
                    Console.WriteLine(ex.Message);
                    resObj = HttpResponseFactory.Build(ResponseCode.BadRequest);
                }
                catch (NotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                    resObj = HttpResponseFactory.Build(ResponseCode.NotFound);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ClientSocketHandler] Caught Error: {ex.Message}");
                    resObj = HttpResponseFactory.Build(ResponseCode.InternalServerError);
                }

                ClientResponseHandler.Handle(streamWriter, resObj);
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}
