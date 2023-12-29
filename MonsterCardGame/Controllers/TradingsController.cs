using DTO;
using MonsterCardGame.Enums;
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
    [ApiController("tradings")]
    public class TradingsController
    {
        private readonly TradeRepository _tradeRepository;
        private readonly CardRepository _cardRepository;
        private readonly CompositeRepository _compositeRepository;
        private readonly Mapper _mapper;

        public TradingsController()
        {
            _tradeRepository = new TradeRepository();
            _cardRepository = new CardRepository();
            _compositeRepository = new CompositeRepository();
            _mapper = new Mapper();
        }

        public TradingsController(
            TradeRepository tradeRepository,
            CardRepository cardRepository,
            CompositeRepository compositeRepository,
            Mapper mapper)
        {
            _tradeRepository = tradeRepository;
            _cardRepository = cardRepository;
            _compositeRepository = compositeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object GetAllOpenTrades()
        {
            IEnumerable<Trade> trades = _tradeRepository.SelectAll();

            List<TradeDTO> tradeDTOs = new List<TradeDTO>();
            foreach (Trade trade in trades)
            {
                tradeDTOs.Add(_mapper.ToDTO(trade));
            }

            return tradeDTOs;
        }

        [HttpPost]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object PostNewTrade([FromBody] CreateTradeDTO createTradeDTO, [FromSession] TokenContent tokenContent)
        {
            Card? card = _cardRepository.SelectById(createTradeDTO.CardToTradeId, tokenContent.UserId);
            if (card == null)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Forbidden,
                    Content = $"Given card '{createTradeDTO.Id}' is not owned by you or does not exist",
                };
            
            if (card.DeckPos != null)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Forbidden,
                    Content = $"Given card to trade is part of a deck",
                };
            
            if (_tradeRepository.IsCardInTrade(card.Id))
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Forbidden,
                    Content = $"Given card is already in a trade",
                };

            if (createTradeDTO.WantedMinDamage < 1)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.BadRequest,
                    Content = $"Minimum damage must be at least 1. Given: {createTradeDTO.WantedMinDamage}",
                };

            if (CardTypeConverter.ToEnum(createTradeDTO.WantedCardType) == CardType.Undefined)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.BadRequest,
                    Content = $"Given wanted card type '{createTradeDTO.WantedCardType}' is not an existing card type",
                };

            bool tradeCreateSuccess = _tradeRepository.Create(
                createTradeDTO.CardToTradeId,
                CardTypeConverter.ToEnum(createTradeDTO.WantedCardType),
                createTradeDTO.WantedMinDamage);

            if (!tradeCreateSuccess)
                return ResponseCode.InternalServerError;

            return ResponseCode.Created;
        }

        [HttpDelete("{tradeid}")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object DeleteTrade([FromPath("tradeid")] int tradeId, [FromSession] TokenContent tokenContent)
        {
            Trade? trade = _tradeRepository.SelectById(tradeId);
            if (trade == null)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.NotFound,
                    Content = $"Trade with id '{tradeId}' does not exist",
                };

            if (trade.OfferedCard.OwnerId != tokenContent.UserId)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Forbidden,
                    Content = $"You do not own the trade offer",
                };

            if (!_tradeRepository.Delete(tradeId))
                return ResponseCode.InternalServerError;

            return ResponseCode.Ok;
        }

        [HttpPost("{tradeid}")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object PostAcceptTrade(
            [FromPath("tradeid")] int tradeId,
            [FromBody] int toTradeCardId,
            [FromSession] TokenContent tokenContent)
        {
            Trade? trade = _tradeRepository.SelectById(tradeId);
            if (trade == null)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.NotFound,
                    Content = $"Trade with id '{tradeId}' does not exist",
                };
            
            if (trade.OfferedCard.OwnerId == tokenContent.UserId)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Forbidden,
                    Content = $"You can't accept your own trades",
                };

            Card? toTradeCard = _cardRepository.SelectById(toTradeCardId, tokenContent.UserId);
            if (toTradeCard == null)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Forbidden,
                    Content = $"To be traded card '{toTradeCardId}' is not owned by you",
                };

            if (trade.WantedCardType != toTradeCard.Type)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Forbidden,
                    Content = $"Wanted type '{CardTypeConverter.ToString(trade.WantedCardType)}' given type '{CardTypeConverter.ToString(toTradeCard.Type)}'",
                };

            if (toTradeCard.Damage < trade.WantedMinDamage)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Forbidden,
                    Content = $"Wanted minimum damage '{trade.WantedMinDamage}' given damage '{toTradeCard.Damage}'",
                };

            if (!_compositeRepository.TradeCards(tradeId, toTradeCard))
                return ResponseCode.InternalServerError;

            return ResponseCode.Ok;
        }
    }
}
