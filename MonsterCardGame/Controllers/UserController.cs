using DTO;
using MonsterCardGame.Middlewares;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
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
    internal class UserController
    {
        private readonly UserRepository _userRepository;
        private readonly Mapper _mapper;

        public UserController()
        {
            _userRepository = new UserRepository();
            _mapper = new Mapper();
        }

        public UserController(UserRepository userRepository, Mapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("{username}")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object Get([FromPath("username")] string username)
        {
            User? user = _userRepository.SelectByUsername(username);

            if (user == null)
                return ResponseCode.NotFound;

            return user;
        }

        [HttpGet("stats")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object GetStats([FromSession] TokenContent tokenContent)
        {
            if (tokenContent == null)
                return ResponseCode.Unauthorized;

            User? user = _userRepository.SelectById(tokenContent.UserId);
            if (user == null)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.InternalServerError,
                    Content = "Could not select user with user id from session",
                };

            return _mapper.ToStats(user);
        }
    }
}
