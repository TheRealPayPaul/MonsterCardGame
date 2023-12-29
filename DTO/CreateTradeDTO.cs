namespace DTO
{
    public class CreateTradeDTO
    {
        public int Id { get; set; }
        public int CardToTradeId { get; set; }
        public string WantedCardType { get; set; } = string.Empty;
        public int WantedMinDamage { get; set; }
    }
}
