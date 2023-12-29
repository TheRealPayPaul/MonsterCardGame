using DTO;
using MonsterCardGame.Game;
using MonsterCardGame.Middlewares;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
using Server.Attributes;
using MonsterCardGame.Models.PJWT;
using Server;
using Server.Enums;

namespace MonsterCardGame.Controllers
{
    [ApiController("game")]
    internal class GameController
    {
        private readonly CardRepository _cardRepository;
        private readonly UserRepository _userRepository;
        private readonly Mapper _mapper;

        public GameController()
        {
            _cardRepository = new CardRepository();
            _userRepository = new UserRepository();
            _mapper = new Mapper();
        }

        public GameController(CardRepository cardRepository, UserRepository userRepository, Mapper mapper)
        {
            _cardRepository = cardRepository;
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
        
        [HttpGet("battles")]
        [ApplyMiddleware(nameof(AuthMiddleware))]
        public object GetBattles([FromSession] TokenContent tokenContent)
        {
            User? user = _userRepository.SelectById(tokenContent.UserId);
            if (user == null)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.InternalServerError,
                    Content = "Could not select user with user id from session",
                };

            List<Card> deck = _cardRepository.SelectAllOfUser(user.Id, SelectOwnerOptions.OnlyDeck).ToList();


            try
            {
                WaitingRoom waitingRoom = BattleOrganizer.Join(user, deck);
                Task.WaitAny(waitingRoom.GameCompletion.Task, Task.Run(() => Task.Delay(Program.ROOM_WAITING_TIME_MS)));

                if (!waitingRoom.GameCompletion.Task.IsCompleted)
                {
                    BattleOrganizer.RemoveWaitingRoom(waitingRoom);
                    return new ActionResult()
                    {
                        ResponseCode = ResponseCode.TimeOut,
                        Content = "Battle didn't start because of timeout",
                    };
                }
                
                BattleLog? battleLog = waitingRoom.GameCompletion.Task.Result;
                if (battleLog == null)
                    return new ActionResult()
                    {
                        ResponseCode = ResponseCode.InternalServerError,
                        Content = "Battle didn't start because of exception",
                    };
                
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Ok,
                    Content = battleLog.GetList(),
                };
            }
            catch (Exception e)
            {
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.InternalServerError,
                    Content = e.Message,
                };
            }
        }
    }
}
