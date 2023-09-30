﻿using Server.Attributes;
using Server.Enums;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utility
{
    internal static class RouteCollector
    {
        public static List<(string, RequestMethod, EndpointChain)> CrawlAssembly(Assembly assembly)
        {
            (List<Type> controllers, List<Type> middlewares) = GatherAllControllersMiddlewares(assembly);
            BuildEndpointChains(controllers, middlewares, out List<(string, RequestMethod, EndpointChain)> endpointChains);

            return endpointChains;
        }

        private static (List<Type>, List<Type>) GatherAllControllersMiddlewares(Assembly assembly)
        {
            List<Type> apiControllers = new();
            List<Type> apiMiddlewares = new();
            foreach (Type type in assembly.GetTypes())
                foreach (CustomAttributeData customAttributeData in type.CustomAttributes)
                {
                    string attributeName = customAttributeData.AttributeType.Name;
                    if (attributeName == nameof(ApiMiddleware))
                    {
                        // Check if IMiddleware interface in ApiMiddleware
                        if (type.GetInterface(nameof(IMiddleware)) == null)
                            throw new Exception($"ApiMiddleware does not implement IMiddleware: {type.Name}");

                        apiMiddlewares.Add(type);
                    }
                    else if (attributeName == nameof(ApiController))
                    {
                        // Check if static ApiControllerClass
                        if (!type.IsAbstract || !type.IsSealed)
                            throw new Exception($"ApiController class is not static: {type.Name}");

                        apiControllers.Add(type);
                    }
                }

            return (apiControllers, apiMiddlewares);
        }

        private static void BuildEndpointChains(List<Type> controllers, List<Type> middlewares, out List<(string, RequestMethod, EndpointChain)> endpointChains)
        {
            endpointChains = new();

            foreach (Type type in controllers)
            {
                ApiController? controllerAttribute = type.GetCustomAttribute(typeof(ApiController)) as ApiController;
                if (controllerAttribute == null)
                    throw new Exception($"Controller attribute could not be casted!");

                foreach (MethodInfo method in type.GetMethods())
                {
                    HttpGet? httpGetAttribute = method.GetCustomAttribute(typeof(HttpGet)) as HttpGet;
                    if (httpGetAttribute != null)
                    {
                        if (method.ReturnType != typeof(HttpResponseObject))
                            throw new Exception($"Endpoint has wrong return Type. {nameof(HttpResponseObject)} needed: {type.Name}; {method.Name}");

                        string path = Path.Combine(controllerAttribute.Path, httpGetAttribute.Path).Replace("\\", "/");
                        EndpointChain endpointChain = new((type, method));
                        endpointChains.Add((path, RequestMethod.GET, endpointChain));
                        continue;
                    }

                    HttpPost? httpPostAttribute = method.GetCustomAttribute(typeof(HttpPost)) as HttpPost;
                    if (httpPostAttribute != null)
                    {
                        string path = Path.Combine(controllerAttribute.Path, httpPostAttribute.Path).Replace("\\", "/");
                        EndpointChain endpointChain = new((type, method));
                        endpointChains.Add((path, RequestMethod.POST, endpointChain));
                        continue;
                    }

                    // TODO PUT and DELETE
                }
            }
        }

    }
}