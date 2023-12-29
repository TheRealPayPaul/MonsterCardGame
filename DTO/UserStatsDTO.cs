namespace DTO
{
    public class UserStatsDTO
    {
        public string Username { get; set; } = string.Empty;
        public int Elo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}
