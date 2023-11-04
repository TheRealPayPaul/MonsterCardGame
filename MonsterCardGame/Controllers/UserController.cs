using DTO;
using MonsterCardGame.Middlewares;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Repositories;
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
    [ApiController("users")]
    internal static class UserController
    {
        [HttpGet("{username}")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public static object Get([FromPath("username")] string username)
        {
            User? user = UserRepository.SelectByUsername(username);

            if (user == null)
                return ResponseCode.NotFound;

            return user;
        }
    }
}
