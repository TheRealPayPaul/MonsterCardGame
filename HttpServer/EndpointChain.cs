using Server.Enums;
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
            // TODO Invoke every middleware
            //foreach ((Type, MethodInfo) middleware in _middlewares)
            //{
            //    // TODO Check if middleware passes
            //    object? instance = Activator.CreateInstance(middleware.Item1);
            //    if (instance == null)
            //    {
            //        throw new Exception("[EndpointChain] Middleware could not be instantiated!");
            //    }

            //    middleware.Item2.Invoke(instance, new[]
            //    {
            //        reqObj,
            //    });
            //}

            HttpResponseObject? resObj = _endpoint.Item2.Invoke(_endpoint.Item1, new[]
            {
                reqObj,
            }) as HttpResponseObject;

            if (resObj == null)
            {
                // TODO return 500
                return new HttpResponseObject();
            }

            return resObj;
        }
    }
}
