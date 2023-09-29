using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Attributes
{
    public class ApplyMiddleware : Attribute
    {
        public readonly Type MiddlewareType;

        public ApplyMiddleware(Type middlewareType)
        {
            MiddlewareType = middlewareType;
        }
    }
}
