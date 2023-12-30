using DTO;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
using Server;
using Server.Attributes;
using Server.Enums;

namespace MonsterCardGame.Controllers                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
{
    [ApiController("auth")]
    public class AuthController
    {
        public const int TTL_MINUTES = 120;

        private readonly UserRepository _userRepository;
        private readonly PJWToken _pjwtoken;
        private readonly Mapper _mapper;

        public AuthController()
        {
            _userRepository = new UserRepository();
            _pjwtoken = new PJWToken();
            _mapper = new Mapper();
        }

        public AuthController(UserRepository userRepository, PJWToken pjwtoken, Mapper mapper)
        {
            _userRepository = userRepository;
            _pjwtoken = pjwtoken;
            _mapper = mapper;
        }


        [HttpPost("login")]
        public object PostLogin([FromBody] UserCredentialsDTO credentials)
        {
            User? user = _userRepository.SelectByCredentials(credentials.Username, credentials.Password);
            if (user == null)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Unauthorized,
                    Content = "Username or Password incorrect"
                };

            return _pjwtoken.CreateToken(_mapper.ToTokenContent(user), DateTime.Now.AddMinutes(TTL_MINUTES), Program.PJWT_SECRET);
        }

        [HttpPost("register")]
        public object PostRegister([FromBody] UserCredentialsDTO credentials)
        {
            if (credentials.Username.Length < Program.MIN_USERNAME_LENGTH)
                return new ActionResult() { ResponseCode = ResponseCode.BadRequest, Content = "Username must be at least 3 characters long" };

            User? existingUser = _userRepository.SelectByUsername(credentials.Username);
            if (existingUser != null)
                return new ActionResult() { ResponseCode = ResponseCode.Conflict, Content = "Username already exists" };

            User user = _userRepository.Create(credentials.Username, credentials.Password, Program.STARTING_COIN_AMOUNT, Program.STARTING_ELO_AMOUNT);

            return _pjwtoken.CreateToken(_mapper.ToTokenContent(user), DateTime.Now.AddMinutes(TTL_MINUTES), Program.PJWT_SECRET);
        }
    }
}
