using Server;
using Server.Attributes;
using Server.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Controllers
{
    [ApiController("User")]
    internal static class UserController
    {
        [HttpGet("{id}")]
        public static HttpResponseObject Get(HttpRequestObject reqObj)
        {
            Console.WriteLine("User Get");
            return new HttpResponseObject();
        }

        [HttpGet]
        public static HttpResponseObject Post(HttpRequestObject reqObj)
        {
            Console.WriteLine("User Post");
            return new HttpResponseObject() { ResponseCode = ResponseCode.ImATeapot };
        }
    }
}
