using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromPath : Attribute
    {
        public readonly string Key;

        public FromPath(string key)
        {
            Key = key;
        }
    }
}
