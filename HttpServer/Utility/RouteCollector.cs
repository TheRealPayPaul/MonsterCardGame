using Server.Attributes;
using Server.Enums;
using Server.Exceptions;
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
            (List<Type> controllers, Dictionary<string, Type> middlewares) = GatherAllControllersMiddlewares(assembly);
            BuildEndpointChains(controllers, middlewares, out List<(string, RequestMethod, EndpointChain)> endpointChains);

            return endpointChains;
        }

        // Return = (apiControllers, apiMiddlewares)
        private static (List<Type>, Dictionary<string, Type>) GatherAllControllersMiddlewares(Assembly assembly)
        {
            List<Type> apiControllers = new();
            Dictionary<string, Type> apiMiddlewares = new();
            foreach (Type type in assembly.GetTypes())
                foreach (CustomAttributeData customAttributeData in type.CustomAttributes)
                {
                    string attributeName = customAttributeData.AttributeType.Name;
                    if (attributeName == nameof(ApiMiddleware))
                    {
                        // Check if IMiddleware interface in ApiMiddleware
                        if (type.GetInterface(nameof(IMiddleware)) == null)
                            throw new InternalServerException($"[{nameof(RouteCollector)}] ApiMiddleware does not implement IMiddleware: {type.Name}");

                        apiMiddlewares.Add(type.Name, type);
                    }
                    else if (attributeName == nameof(ApiController))
                    {
                        // Check if static ApiControllerClass
                        if (!type.IsAbstract || !type.IsSealed)
                            throw new InternalServerException($"[{nameof(RouteCollector)}] ApiController class is not static: {type.Name}");

                        apiControllers.Add(type);
                    }
                }

            return (apiControllers, apiMiddlewares);
        }

        private static void BuildEndpointChains(List<Type> controllers, Dictionary<string, Type> middlewares, out List<(string, RequestMethod, EndpointChain)> endpointChains)
        {
            endpointChains = new();

            foreach (Type type in controllers)
            {
                ApiController? controllerAttribute = type.GetCustomAttribute(typeof(ApiController)) as ApiController;
                if (controllerAttribute == null)
                    throw new InternalServerException($"[{nameof(RouteCollector)}] Controller attribute could not be casted!");

                foreach (MethodInfo method in type.GetMethods())
                {
                    // TODO das geht sicherlich besser.
                    // Problem sind die vielen Variabeln die benötigt werden.
                    HttpGet? httpGetAttribute = method.GetCustomAttribute(typeof(HttpGet)) as HttpGet;
                    ApplyMiddleware? applyMiddlewareAttribute = method.GetCustomAttribute(typeof(ApplyMiddleware)) as ApplyMiddleware;
                    if (httpGetAttribute != null)
                    {
                        string path = Path.Combine(controllerAttribute.Path, httpGetAttribute.Path).Replace("\\", "/");
                        EndpointChain endpointChain = new((type, method));
                        if (applyMiddlewareAttribute != null)
                        {
                            foreach (string middlewareName in applyMiddlewareAttribute.MiddlewareNames)
                            {
                                middlewares.TryGetValue(middlewareName, out Type? middlewareType);
                                if (middlewareType == null)
                                    throw new InternalServerException($"[{nameof(RouteCollector)}] Middleware with the name '{middlewareName}' doesn't exist!");
                                
                                MethodInfo? middlewareMethodInfo = middlewareType.GetMethod("Invoke");
                                if (middlewareMethodInfo == null)
                                    throw new InternalServerException($"[{nameof(RouteCollector)}] Middleware named '{middlewareName}' has no function named 'Invoke'");

                                endpointChain.AddMiddleware(middlewareType, middlewareMethodInfo);
                            }
                        }

                        endpointChains.Add((path, RequestMethod.GET, endpointChain));
                        continue;
                    }

                    HttpPost? httpPostAttribute = method.GetCustomAttribute(typeof(HttpPost)) as HttpPost;
                    if (httpPostAttribute != null)
                    {
                        if (method.ReturnType != typeof(HttpResponseObject))
                            throw new InternalServerException($"[{nameof(RouteCollector)}] Endpoint has wrong return Type. '{nameof(HttpResponseObject)}' needed. Current: {type.Name}; {method.Name}");

                        string path = Path.Combine(controllerAttribute.Path, httpPostAttribute.Path).Replace("\\", "/");
                        EndpointChain endpointChain = new((type, method));
                        endpointChains.Add((path, RequestMethod.POST, endpointChain));
                        continue;
                    }

                    HttpPut? httpPutAttribute = method.GetCustomAttribute(typeof(HttpPut)) as HttpPut;
                    if (httpPutAttribute != null)
                    {
                        if (method.ReturnType != typeof(HttpResponseObject))
                            throw new InternalServerException($"[{nameof(RouteCollector)}] Endpoint has wrong return Type. '{nameof(HttpResponseObject)}' needed. Current: {type.Name}; {method.Name}");

                        string path = Path.Combine(controllerAttribute.Path, httpPutAttribute.Path).Replace("\\", "/");
                        EndpointChain endpointChain = new((type, method));
                        endpointChains.Add((path, RequestMethod.PUT, endpointChain));
                        continue;
                    }

                    HttpDelete? httpDeleteAttribute = method.GetCustomAttribute(typeof(HttpDelete)) as HttpDelete;
                    if (httpDeleteAttribute != null)
                    {
                        if (method.ReturnType != typeof(HttpResponseObject))
                            throw new InternalServerException($"[{nameof(RouteCollector)}] Endpoint has wrong return Type. '{nameof(HttpResponseObject)}' needed. Current: {type.Name}; {method.Name}");

                        string path = Path.Combine(controllerAttribute.Path, httpDeleteAttribute.Path).Replace("\\", "/");
                        EndpointChain endpointChain = new((type, method));
                        endpointChains.Add((path, RequestMethod.DELETE, endpointChain));
                        continue;
                    }
                }
            }
        }
    }
}
