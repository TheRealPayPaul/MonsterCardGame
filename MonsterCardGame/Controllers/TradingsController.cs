using DTO;
using MonsterCardGame.Middlewares;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
using Server.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Controllers
{
    [ApiController("tradings")]
    internal class TradingsController
    {
        private readonly TradeRepository _tradeRepository;
        private readonly Mapper _mapper;

        public TradingsController()
        {
            _tradeRepository = new TradeRepository();
            _mapper = new Mapper();
        }

        public TradingsController(TradeRepository tradeRepository, Mapper mapper)
        {
            _tradeRepository = tradeRepository;
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
        public object PostNewTrade()
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{tradeid}")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object DeleteTrade()
        {
            throw new NotImplementedException();
        }

        [HttpPost("{tradeid}")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object PostAcceptTrade()
        {
            throw new NotImplementedException();
        }
    }
}
