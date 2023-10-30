using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ApplyMiddleware : Attribute
    {
        public readonly IEnumerable<string> MiddlewareNames;

        public ApplyMiddleware(string firstMiddlewareName, params string[] middlewareNames)
        {
            MiddlewareNames = new[] { firstMiddlewareName }.Concat(middlewareNames);
        }
    }
}
