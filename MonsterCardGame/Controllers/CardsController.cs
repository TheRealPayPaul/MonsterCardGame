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
    [ApiController("cards")]
    public class CardsController
    {
        private readonly TradeRepository _tradeRepository;
        private readonly CardRepository _cardRepository;
        private readonly Mapper _mapper;

        public CardsController()
        {
            _tradeRepository = new TradeRepository();
            _cardRepository = new CardRepository();
            _mapper = new Mapper();
        }

        public CardsController(TradeRepository tradeRepository, CardRepository cardRepository, Mapper mapper)
        {
            _tradeRepository = tradeRepository;
            _cardRepository = cardRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object GetCards([FromSession] TokenContent tokenContent)
        {
            return _mapper.ToDTO(_cardRepository.SelectAllOfUser(tokenContent.UserId).ToArray());
        }

        [HttpGet("deck")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object GetDeck([FromSession] TokenContent tokenContent)
        {
            return _mapper.ToDTO(_cardRepository.SelectAllOfUser(tokenContent.UserId, SelectOwnerOptions.OnlyDeck).ToArray());
        }

        [HttpPut("deck")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object PutDeck([FromSession] TokenContent tokenContent, [FromBody] int[] cardIds)
        {
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

            foreach (int cardId in cardIds)
            {
                if (_tradeRepository.IsCardInTrade(cardId))
                    return new ActionResult()
                    {
                        ResponseCode = ResponseCode.BadRequest,
                        Content = $"Card with id: '{cardId}' is in a trade",
                    };
            }
            
            Card[] cards = new Card[Program.DECK_SIZE];
            for (int i = 0; i < Program.DECK_SIZE; i++)
            {
                Card? card = _cardRepository.SelectById(cardIds[i], tokenContent.UserId);
                if (card == null)
                    return new ActionResult()
                    {
                        ResponseCode = ResponseCode.Forbidden,
                        Content = $"At least one of the provided cards does not belong to the user or is not available.",
                    };

                cards[i] = card;
            }

            return _cardRepository.UpdateDeck(cards) ? ResponseCode.Ok : ResponseCode.Forbidden;
        }
    }
}
