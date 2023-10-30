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
        public static void Handle(TcpClient client, RequestTree requestTree)
        {
            using (StreamWriter streamWriter = new(client.GetStream()) { AutoFlush = true })
            using (StreamReader streamReader = new(client.GetStream()))
            {
                HttpResponseObject resObj;
                try
                {
                    HttpRequestObject reqObj = ClientRequestHandler.Handle(streamReader);

                    EndpointChain? endpointChain = requestTree.GetEndpoint(reqObj);
                    if (endpointChain == null)
                        throw new NotFoundException($"[ClientSocketHandler] EndpointChain Not Found: {reqObj.RequestType} {reqObj.Path}");

                    object? endpointResult = endpointChain.Invoke(reqObj);
                    resObj = HttpResponseFactory.Build(endpointResult);
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
                    Console.WriteLine($"[ClientSocketHandler] Caught Error: {ex.Message}\n{ex.StackTrace} ");
                    resObj = HttpResponseFactory.Build(ResponseCode.InternalServerError);
                }

                ClientResponseHandler.Handle(streamWriter, resObj);
            }

            client.Dispose();
        }
    }
}
