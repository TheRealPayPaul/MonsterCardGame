using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPut : Attribute, IHttpMethod
    {
        public readonly string Path;

        public HttpPut(string path = "")
        {
            Path = path;
        }

        public string GetPath()
        {
            return Path;
        }
    }
}
