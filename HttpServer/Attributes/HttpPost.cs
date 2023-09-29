using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Attributes
{
    public class HttpPost : Attribute
    {
        public readonly string Path;

        public HttpPost(string path)
        {
            Path = path;
        }

        public HttpPost()
        {
            Path = "";
        }
    }
}
