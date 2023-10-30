using DTO;
using MonsterCardGame.Models;
using MonsterCardGame.Repositories;
using MonsterCardGame.Uitilities;
using Server;
using Server.Attributes;
using Server.Enums;

namespace MonsterCardGame.Controllers
{
    [ApiController("Auth")]
    public static class AuthController
    {
        [HttpPost("Login")]
        public static object PostLogin([FromBody] UserCredentials credentials)
        {
            try
            {
                User? user = UserRepository.SelectByCredentials(credentials.Username, credentials.Password);
                if (user == null)
                    return new ActionResult() { ResponseCode = ResponseCode.BadRequest };

                return PJWToken.CreateToken(user, DateTime.Now.AddMinutes(10), Program.PJWT_SECRET);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{nameof(AuthController)}] Caught excpetion Login: {ex.Message}");
                return new ActionResult() { ResponseCode = ResponseCode.InternalServerError };
            }
        }

        [HttpPost("Register")]
        public static object PostRegister([FromBody] UserCredentials credentials)
        {
            try
            {
                if (credentials.Username.Length < 3)
                    return new ActionResult() { ResponseCode = ResponseCode.BadRequest, Content = "Username must be at least 3 characters long" };

                User? existingUser = UserRepository.SelectByUsername(credentials.Username);
                if (existingUser != null)
                    return new ActionResult() { ResponseCode = ResponseCode.BadRequest, Content = "Username already exists" };

                User user = UserRepository.Create(credentials.Username, credentials.Password, Program.STARTING_COIN_AMOUNT);

                return PJWToken.CreateToken(user, DateTime.Now.AddMinutes(10), Program.PJWT_SECRET);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{nameof(AuthController)}] Caught excpetion Register: {ex.Message}");
                return new ActionResult() { ResponseCode = ResponseCode.InternalServerError };
            }
        }
    }
}
