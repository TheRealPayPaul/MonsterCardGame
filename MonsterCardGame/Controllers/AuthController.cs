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
    internal static class AuthController
    {
        public const int TTL_MINUTES = 120;

        [HttpPost("login")]
        public static object PostLogin([FromBody] UserCredentialsDTO credentials)
        {
            User? user = UserRepository.SelectByCredentials(credentials.Username, credentials.Password);
            if (user == null)
                return new ActionResult() { ResponseCode = ResponseCode.BadRequest };

            return PJWToken.CreateToken(Mapper.ToTokenContent(user), DateTime.Now.AddMinutes(TTL_MINUTES), Program.PJWT_SECRET);
        }

        [HttpPost("register")]
        public static object PostRegister([FromBody] UserCredentialsDTO credentials)
        {
            if (credentials.Username.Length < 3)
                return new ActionResult() { ResponseCode = ResponseCode.BadRequest, Content = "Username must be at least 3 characters long" };

            User? existingUser = UserRepository.SelectByUsername(credentials.Username);
            if (existingUser != null)
                return new ActionResult() { ResponseCode = ResponseCode.Conflict, Content = "Username already exists" };

            User user = UserRepository.Create(credentials.Username, credentials.Password, Program.STARTING_COIN_AMOUNT);

            return PJWToken.CreateToken(Mapper.ToTokenContent(user), DateTime.Now.AddMinutes(TTL_MINUTES), Program.PJWT_SECRET);
        }
    }
}
