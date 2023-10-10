using Server.Enums;
using Server.Exceptions;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class EndpointChain
    {
        private List<(Type, MethodInfo)> _middlewares = new();
        private (Type, MethodInfo) _endpoint;

        public EndpointChain((Type, MethodInfo) endpoint)
        {
            _endpoint = endpoint;
        }

        public void AddMiddleware(Type type, MethodInfo methodInfo)
        {
            _middlewares.Add((type, methodInfo));
        }

        public HttpResponseObject Invoke(HttpRequestObject reqObj)
        {
            // Middleware
            foreach ((Type, MethodInfo) middleware in _middlewares)
            {
                object? instance = Activator.CreateInstance(middleware.Item1);
                if (instance == null)
                {
                    throw new InternalServerException("[EndpointChain] Middleware could not be instantiated!");
                }

                HttpResponseObject? midResObj = middleware.Item2.Invoke(instance, new[]
                {
                    reqObj,
                }) as HttpResponseObject;

                if (midResObj != null)
                    return midResObj;
            }

            // Endpoint
            HttpResponseObject? resObj = _endpoint.Item2.Invoke(_endpoint.Item1, new[]
            {
                reqObj,
            }) as HttpResponseObject;

            if (resObj == null)
                throw new InternalServerException("[EndpointChain] No HttpResponseObject from Endpoint!");

            return resObj;
        }
    }
}
