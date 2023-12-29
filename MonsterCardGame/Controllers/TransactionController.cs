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
    internal class TransactionController
    {
        private readonly UserRepository _userRepository;
        private readonly CompositeRepository _compositeRepository;
        private readonly Mapper _mapper;

        public TransactionController()
        {
            _userRepository = new UserRepository();
            _compositeRepository = new CompositeRepository();
            _mapper = new Mapper();
        }

        public TransactionController(
            UserRepository userRepository,
            CompositeRepository compositeRepository,
            Mapper mapper)
        {
            _userRepository = userRepository;
            _compositeRepository = compositeRepository;
            _mapper = mapper;
        }

        [HttpPost("packages")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object Packages([FromSession] TokenContent tokenContent)
        {
            User? user = _userRepository.SelectById(tokenContent.UserId);
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

            if (!_compositeRepository.BuyCards(cards, Program.PACKAGE_COST, tokenContent.UserId))
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.InternalServerError,
                    Content = "Could not perform necessary DB actions",
                };

            return _mapper.ToDTO(cards);
        }
    }
}
