﻿using Server;
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
        [ApplyMiddleware("AuthMiddleware")]
        public static object Get([RawHttpRequest] HttpRequestObject reqObj, [FromPath("id")] int id)
        {
            Console.WriteLine($"User Get id: {id} {reqObj.Path}");
            return new ActionResult() { ResponseCode = ResponseCode.ImATeapot, Content = reqObj };
        }

        [HttpGet]
        public static void Post(HttpRequestObject reqObj)
        {
            Console.WriteLine("User Post");
            //return new HttpResponseObject() { ResponseCode = ResponseCode.ImATeapot };
        }
    }
}
