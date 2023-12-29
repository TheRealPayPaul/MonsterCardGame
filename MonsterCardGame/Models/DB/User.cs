namespace MonsterCardGame.Models.DB
{
    public class User
    {
        public int Id { get; set; } = -1;
        public string Username { get; set; } = string.Empty;
        public int Coins { get; set; }
        public int Elo { get; set; } = 100;
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}
