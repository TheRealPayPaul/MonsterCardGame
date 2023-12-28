using MonsterCardGame.Models.DB;

namespace MonsterCardGame.Game;

public class WaitingRoom
{
    public User Player1 { get; }
    public List<Card> DeckPlayer1 { get; }
    public User? Player2 { get; set; }
    public List<Card>? DeckPlayer2 { get; set; }
    public TaskCompletionSource<BattleLog?> GameCompletion { get; }

    public WaitingRoom(User player1, List<Card> deckPlayer1)
    {
        Player1 = player1;
        DeckPlayer1 = deckPlayer1;
        GameCompletion = new TaskCompletionSource<BattleLog?>();
    }

    public bool IsRoomReady()
    {
        return Player2 != null && DeckPlayer2 != null;
    }
}