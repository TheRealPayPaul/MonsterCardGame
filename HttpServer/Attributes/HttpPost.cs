using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPost : Attribute
    {
        public readonly string Path;

        public HttpPost(string path = "")
        {
            Path = path;
        }
    }
}
