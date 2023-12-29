using MonsterCardGame.Middlewares;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Repositories;
using Server;
using Server.Attributes;
using Server.Enums;

namespace MonsterCardGame.Controllers;

[ApiController("casino")]
public class CasinoController
{
    private readonly UserRepository _userRepository;

    public CasinoController()
    {
        _userRepository = new UserRepository();
    }

    public CasinoController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpPost("coinflip")]
    [ApplyMiddleware(nameof(AuthMiddleware))]
    public object PostCoinflip([FromSession] TokenContent tokenContent, [FromBody] int amount)
    {
        User? user = _userRepository.SelectById(tokenContent.UserId);
        if (user == null)
            return new ActionResult()
            {
                ResponseCode = ResponseCode.InternalServerError,
                Content = $"User with id: {tokenContent.UserId} not found",
            };

        if (amount > user.Coins)
            return new ActionResult()
            {
                ResponseCode = ResponseCode.BadRequest,
                Content = $"Amount ({amount}) higher than actual coin count ({user.Coins})",
            };

        bool dbQueryResult;
        string returnMessage;
        Random random = new();
        if (random.Next(2) == 1)
        {
            int newAmount = user.Coins + amount * 2;
            dbQueryResult = _userRepository.SetCoins(user.Id, newAmount);
            returnMessage = $"Won the coinflip new balance: {newAmount}";
        }
        else
        {
            int newAmount = user.Coins - amount;
            dbQueryResult = _userRepository.SetCoins(user.Id, newAmount);
            returnMessage = $"Lost the coinflip new balance: {newAmount}";
        }
        
        if (!dbQueryResult)
            return new ActionResult()
            {
                ResponseCode = ResponseCode.InternalServerError,
                Content = $"Coins could not be updated in DB",
            };
        
        return new ActionResult()
        {
            ResponseCode = ResponseCode.Ok,
            Content = returnMessage,
        };
    }
}