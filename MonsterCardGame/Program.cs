using Server;
using System.Net;

namespace MonsterCardGame
{
    public class Program
    {
        public const int ROOM_WAITING_TIME_MS = 30 * 1000; // Milliseconds
        public const int MAX_LOOP_ITERATIONS_BATLLE = 100;
        public const string CONNECTION_STRING = "Host=localhost;Database=MonsterCardGame;Username=admin;Password=admin;Persist Security Info=True";
        public const int STARTING_COIN_AMOUNT = 20;
        public const int STARTING_ELO_AMOUNT = 100;
        public const string PJWT_SECRET = "SuperSecret";
        public const int MIN_DAMAGE = 40;
        public const int MAX_DAMAGE = 90;
        public const int PACKAGE_COST = 5;
        public const int DECK_SIZE = 4;

        static void Main(string[] args)
        {
            HttpServer server = new(IPAddress.Loopback, 8080);
            server.SetMaxConnections(50);
            server.MapControllers();
            server.Start();
        }
    }
}