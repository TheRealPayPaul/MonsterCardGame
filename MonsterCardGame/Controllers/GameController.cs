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
    [ApiController("game")]
    internal class GameController
    {
        private readonly UserRepository _userRepository;
        private readonly Mapper _mapper;

        public GameController()
        {
            _userRepository = new UserRepository();
            _mapper = new Mapper();
        }

        public GameController(UserRepository userRepository, Mapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("scoreboard")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object GetScoreboard()
        {
            IEnumerable<User> users = _userRepository.SelectAll();

            List<UserStatsDTO> scoreboardData = new();
            foreach (User user in users)
            {
                scoreboardData.Add(_mapper.ToStats(user));
            }

            return scoreboardData;
        }
    }
}
