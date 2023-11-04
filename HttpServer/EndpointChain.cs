using Server.Attributes;
using Server.Enums;
using Server.Exceptions;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
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

        public object? Invoke(HttpRequestObject reqObj)
        {
            // Middleware
            foreach ((Type, MethodInfo) middleware in _middlewares)
            {
                object? instance = Activator.CreateInstance(middleware.Item1);
                if (instance == null)
                {
                    throw new InternalServerException($"[{nameof(EndpointChain)}] Middleware could not be instantiated!");
                }

                object? midResObj = middleware.Item2.Invoke(instance, GetParameters(middleware.Item2, reqObj));

                if (midResObj != null)
                    return midResObj; 
            }

            // Endpoint
            object? resObj = _endpoint.Item2.Invoke(null, GetParameters(_endpoint.Item2, reqObj));

            return resObj;
        }

        private object?[] GetParameters(MethodInfo methodInfo, HttpRequestObject reqObj)
        {
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            object?[] parameters = new object[parameterInfos.Length];

            for (int index=0; index < parameterInfos.Length; index++)
            {
                ParameterInfo parameterInfo = parameterInfos[index];

                FromBody? fromBody = parameterInfo.GetCustomAttribute<FromBody>();
                if (fromBody != null)
                {
                    if (parameterInfo.ParameterType == typeof(string))
                    {
                        parameters[index] = reqObj.RawBody;
                        continue;
                    }

                    parameters[index] = JsonSerializer.Deserialize(reqObj.RawBody, parameterInfo.ParameterType);
                    continue;
                }

                FromPath? fromPath = parameterInfo.GetCustomAttribute<FromPath>();
                if (fromPath != null)
                {
                    reqObj.DynamicPathParameters.TryGetValue(fromPath.Key, out string? value);
                    if (value == null)
                        continue;

                    if (parameterInfo.ParameterType == typeof(string))
                    {
                        parameters[index] = value;
                        continue;
                    }

                    parameters[index] = JsonSerializer.Deserialize(value, parameterInfo.ParameterType);
                    continue;
                }

                FromRequest? fromRequest = parameterInfo.GetCustomAttribute<FromRequest>();
                if (fromRequest != null)
                {
                    reqObj.RequestParameters.TryGetValue(fromRequest.Key, out string? value);
                    if (value == null)
                        continue;

                    if (parameterInfo.ParameterType == typeof(string))
                    {
                        parameters[index] = value;
                        continue;
                    }

                    parameters[index] = JsonSerializer.Deserialize(value, parameterInfo.ParameterType);
                    continue;
                }

                FromSession? fromSession = parameterInfo.GetCustomAttribute<FromSession>();
                if (fromSession != null)
                {
                    parameters[index] = reqObj.SessionContent;
                    continue;
                }

                RawHttpRequest? rawHttpRequest = parameterInfo.GetCustomAttribute<RawHttpRequest>();
                if (rawHttpRequest != null)
                {
                    if (parameterInfo.ParameterType != typeof(HttpRequestObject))
                        throw new InternalServerException($"[{nameof(EndpointChain)}]");

                    parameters[index] = reqObj;
                    continue;
                }

                throw new InternalServerException($"[{nameof(EndpointChain)}] Every parameter of a middleware or controller method needs to have an attribute! {methodInfo.DeclaringType?.Name}");
            }

            return parameters;
        }
    }
}
