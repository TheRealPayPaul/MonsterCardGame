using Server;
using Server.Attributes;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Middlewares
{
    [ApiMiddleware]
    internal class AuthMiddleware : IMiddleware
    {
        public HttpResponseObject? Invoke([RawHttpRequest] HttpRequestObject reqObj)
        {
            Console.WriteLine("AUTH YAY");
            reqObj.Path = "DADADADAWDAWD";
            return null;
        }
    }
}
