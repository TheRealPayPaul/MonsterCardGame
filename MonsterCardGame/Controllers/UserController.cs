using DTO;
using MonsterCardGame.Middlewares;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
using Server;
using Server.Attributes;
using Server.Enums;

namespace MonsterCardGame.Controllers
{
    [ApiController("users")]
    public class UserController
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
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.NotFound,
                    Content = $"User with name '{username}' not found",
                };

            return user;
        }

        [HttpPut]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object Put([FromSession] TokenContent tokenContent, [FromBody] UserDataDTO userData)
        {
            if (userData.Username.Length < Program.MIN_USERNAME_LENGTH)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.BadRequest,
                    Content = "Username must be at least 3 characters long",
                };
            
            if (_userRepository.SelectByUsername(userData.Username) != null)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.BadRequest,
                    Content = $"Username: {userData.Username} already taken",
                };

            if (!_userRepository.UpdateUsername(tokenContent.UserId, userData.Username))
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.InternalServerError,
                    Content = $"Could not change user data",
                };

            return ResponseCode.Ok;
        }

        [HttpGet("stats")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object GetStats([FromSession] TokenContent tokenContent)
        {
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
