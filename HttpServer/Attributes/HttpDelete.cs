using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Attributes
{
    public class HttpDelete : Attribute
    {
        public readonly string Path;

        public HttpDelete(string path = "")
        {
            Path = path;
        }
    }
}
