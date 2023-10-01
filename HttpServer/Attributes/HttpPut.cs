﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Attributes
{
    public class HttpPut : Attribute
    {
        public readonly string Path;

        public HttpPut(string path = "")
        {
            Path = path;
        }
    }
}
