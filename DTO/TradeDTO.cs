namespace DTO
{
    public class TradeDTO
    {
        public int Id { get; set; }
        public CardDTO CardToTrade { get; set; } = new();
        public string WantedCardType { get; set; } = string.Empty;
        public int WantedMinDamage { get; set; }
    }
}
