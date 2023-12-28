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
    [ApiController("cards")]
    internal class CardsController
    {
        private readonly CardRepository _cardRepository;
        private readonly Mapper _mapper;

        public CardsController()
        {
            _cardRepository = new CardRepository();
            _mapper = new Mapper();
        }

        [HttpGet]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object GetCards([FromSession] TokenContent tokenContent)
        {
            if (tokenContent == null)
                return ResponseCode.Unauthorized;

            return _mapper.ToDTO(_cardRepository.SelectAllOfUser(tokenContent.UserId).ToArray());
        }

        [HttpGet("deck")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object GetDeck([FromSession] TokenContent tokenContent)
        {
            if (tokenContent == null)
                return ResponseCode.Unauthorized;

            return _mapper.ToDTO(_cardRepository.SelectAllOfUser(tokenContent.UserId, SelectOwnerOptions.OnlyDeck).ToArray());
        }

        [HttpPut("deck")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object PutDeck([FromSession] TokenContent tokenContent, [FromBody] int[] cardIds)
        {
            if (tokenContent == null)
                return ResponseCode.Unauthorized;

            if (cardIds.Length != Program.DECK_SIZE)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.BadRequest,
                    Content = $"Exactly {Program.DECK_SIZE} card_ids must be provided",
                };

            if (cardIds.Distinct().Count() != Program.DECK_SIZE)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.BadRequest,
                    Content = $"Deck holds at least one card multiple times",
                };
            
            Card[] cards = new Card[Program.DECK_SIZE];
            for (int i = 0; i < Program.DECK_SIZE; i++)
            {
                Card? card = _cardRepository.SelectById(cardIds[i], tokenContent.UserId);
                if (card == null)
                    return ResponseCode.Forbidden;

                cards[i] = card;
            }

            return _cardRepository.UpdateDeck(cards) ? ResponseCode.Ok : ResponseCode.Forbidden;
        }
    }
}
