using Server.Attributes;
using Server.Enums;
using Server.Exceptions;
using Server.Interfaces;
using System.Reflection;

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
                        apiControllers.Add(type);
                    }
                }

            return (apiControllers, apiMiddlewares);
        }

        private static void BuildEndpointChains(List<Type> controllers, Dictionary<string, Type> middlewares, out List<(string, RequestMethod, EndpointChain)> endpointChains)
        {
            endpointChains = new();

            foreach (Type controllerType in controllers)
            {
                ApiController? controllerAttribute = controllerType.GetCustomAttribute(typeof(ApiController)) as ApiController;
                if (controllerAttribute == null)
                    throw new InternalServerException($"[{nameof(RouteCollector)}] Controller attribute could not be casted!");

                foreach (MethodInfo method in controllerType.GetMethods())
                {
                    EndpointChainParameters parameters = new()
                    {
                        method = method,
                        controllerAttribute = controllerAttribute,
                        controllerType = controllerType,
                        endpointChains = endpointChains,
                        middlewares = middlewares,
                    };

                    parameters.methodAttributeType = typeof(HttpGet);
                    parameters.requestMethod = RequestMethod.GET;
                    if (BuildEndpointChain(parameters)) continue;

                    parameters.methodAttributeType = typeof(HttpPost);
                    parameters.requestMethod = RequestMethod.POST;
                    if (BuildEndpointChain(parameters)) continue;

                    parameters.methodAttributeType = typeof(HttpPut);
                    parameters.requestMethod = RequestMethod.PUT;
                    if (BuildEndpointChain(parameters)) continue;

                    parameters.methodAttributeType = typeof(HttpDelete);
                    parameters.requestMethod = RequestMethod.DELETE;
                    BuildEndpointChain(parameters);
                }
            }
        }

        struct EndpointChainParameters
        {
            public Type methodAttributeType;
            public RequestMethod requestMethod;

            public MethodInfo method;
            public ApiController controllerAttribute;
            public Type controllerType;
            public List<(string, RequestMethod, EndpointChain)> endpointChains;
            public Dictionary<string, Type> middlewares;
        }

        private static bool BuildEndpointChain(EndpointChainParameters parameters)
        {
            IHttpMethod? httpMethodAttribute = parameters.method.GetCustomAttribute(parameters.methodAttributeType) as IHttpMethod;
            ApplyMiddleware? applyMiddlewareAttribute = parameters.method.GetCustomAttribute(typeof(ApplyMiddleware)) as ApplyMiddleware;
            if (httpMethodAttribute != null)
            {
                string path = Path.Combine(parameters.controllerAttribute.Path, httpMethodAttribute.GetPath()).Replace("\\", "/");
                EndpointChain endpointChain = new((parameters.controllerType, parameters.method));
                if (applyMiddlewareAttribute != null)
                {
                    foreach (string middlewareName in applyMiddlewareAttribute.MiddlewareNames)
                    {
                        parameters.middlewares.TryGetValue(middlewareName, out Type? middlewareType);
                        if (middlewareType == null)
                            throw new InternalServerException($"[{nameof(RouteCollector)}] Middleware with the name '{middlewareName}' doesn't exist!");

                        MethodInfo? middlewareMethodInfo = middlewareType.GetMethod("Invoke");
                        if (middlewareMethodInfo == null)
                            throw new InternalServerException($"[{nameof(RouteCollector)}] Middleware named '{middlewareName}' has no function named 'Invoke'");

                        endpointChain.AddMiddleware(middlewareType, middlewareMethodInfo);
                    }
                }

                parameters.endpointChains.Add((path, parameters.requestMethod, endpointChain));
                return true;
            }

            return false;
        }
    }
}
