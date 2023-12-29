using MonsterCardGame.Models.DB;

namespace MonsterCardGame.Game;

public static class BattleOrganizer
{
    private static List<WaitingRoom> _waitingRooms = new();
    
    public static WaitingRoom Join(User user, List<Card> deck)
    {
        if (!DeckIsValid(deck))
            throw new ArgumentException("Given deck is not valid");
        
        lock (_waitingRooms)
        {
            WaitingRoom? roomWithUser = _waitingRooms
                .Where(room => room.Player1.Id == user.Id || room.Player2?.Id == user.Id)
                .FirstOrDefault();

            if (roomWithUser != null)
            {
                return roomWithUser;
            }
            
            List<WaitingRoom> openRooms = _waitingRooms
                .Where(room => room.IsRoomReady() == false)
                .ToList();
            
            WaitingRoom roomToJoin;
            if (openRooms.Count > 0)
            {
                roomToJoin = openRooms.First();
                roomToJoin.Player2 = user;
                roomToJoin.DeckPlayer2 = deck;

                Task.Run(() => StartBattle(roomToJoin));
            }
            else
            {
                roomToJoin = new WaitingRoom(user, deck);
                _waitingRooms.Add(roomToJoin);
            }
            
            return roomToJoin;
        }
    }

    public static void StartBattle(WaitingRoom room)
    {
        try
        {
            BattleManager battleManager = new(room);
            battleManager.StartBattle();
        }
        catch (Exception e)
        {
            Console.WriteLine($"(BattleOrganizer) Exception occured: {e.Message}"); 
            room.GameCompletion.SetResult(null);
        }
    }

    public static void RemoveWaitingRoom(WaitingRoom room)
    {
        // Don't remove when game is going on
        if (room.IsRoomReady())
            return;
        
        lock (_waitingRooms)
        {
            _waitingRooms.Remove(room);
        }
    }
    
    public static void RemoveWaitingRoomForce(WaitingRoom room)
    {
        lock (_waitingRooms)
        {
            _waitingRooms.Remove(room);
        }
    }

    private static bool DeckIsValid(List<Card> deck)
    {
        if (deck.Count != Program.DECK_SIZE)
            return false;
        
        return deck.Where(d => d.DeckPos == null).Count() == 0;
    }
}