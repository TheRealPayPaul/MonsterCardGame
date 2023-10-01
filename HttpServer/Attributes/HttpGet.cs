using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Attributes
{
    public class HttpGet : Attribute
    {
        public readonly string Path;

        public HttpGet(string path = "")
        {
            Path = path;
        }
    }
}
