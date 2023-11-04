using MonsterCardGame.Factories;
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
    [ApiController("transactions")]
    internal static class TransactionController
    {
        [HttpPost("packages")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public static object Packages([FromSession] TokenContent tokenContent)
        {
            if (tokenContent == null)
                return ResponseCode.Unauthorized;

            User? user = UserRepository.SelectById(tokenContent.UserId);
            if (user == null)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.InternalServerError,
                    Content = "Could not select user with user id from session",
                };

            if (user.Coins < Program.PACKAGE_COST)
                return ResponseCode.Forbidden;

            Card[] cards = new Card[5];
            for (int i = 0; i < cards.Length; i++)
                cards[i] = CardFactory.Random();

            if (!CompositeRepository.BuyCards(cards, Program.PACKAGE_COST, tokenContent.UserId))
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.InternalServerError,
                    Content = "Could not perform necessary DB actions",
                };

            return cards;
        }
    }
}
