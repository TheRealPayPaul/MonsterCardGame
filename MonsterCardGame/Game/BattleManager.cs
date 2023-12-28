using MonsterCardGame.Enums;
using MonsterCardGame.Models.DB;

namespace MonsterCardGame.Game;

public class BattleManager
{
    private WaitingRoom _waitingRoom;
    private BattleLog _battleLog = new();
    private TaskCompletionSource<BattleLog?> _gameCompletion;

    private User _player1;
    private User _player2;
    
    private List<Card> _deckPlayer1;
    private List<Card> _deckPlayer2;

    private int _currentRound;

    public BattleManager(WaitingRoom room)
    {
        _waitingRoom = room;
        _gameCompletion = room.GameCompletion;

        _player1 = room.Player1;
        _player2 = room.Player2 ?? throw new ArgumentException("Player 2 is null");
        
        _deckPlayer1 = room.DeckPlayer1;
        _deckPlayer2 = room.DeckPlayer2 ?? throw new ArgumentException("Player Deck 2 is null");
    }

    public void StartBattle()
    {
        for (int i = 0; i < Program.MAX_LOOP_ITERATIONS_BATLLE; i++)
        {
            _currentRound = i;
                
            BattleRound();
            if (IsBattleOver())
                break;
        }
        
        // Update decks in DB
        // Update Elo and User Stats
        // Determin winner/loser/draw and add to log
        _gameCompletion.SetResult(_battleLog);
        BattleOrganizer.RemoveWaitingRoom(_waitingRoom);
    }

    private bool IsBattleOver()
    {
        return _deckPlayer1.Count < 1 || _deckPlayer2.Count < 1;
    }
    
    private void BattleRound()
    {
        Card cardPlayer1 = RandomCardFromDeck(_deckPlayer1);
        Card cardPlayer2 = RandomCardFromDeck(_deckPlayer2);

        float cardPlayer1Dmg = CalculateCardDamage(cardPlayer1, cardPlayer2);
        float cardPlayer2Dmg = CalculateCardDamage(cardPlayer2, cardPlayer1);

        if (Math.Abs(cardPlayer1Dmg - cardPlayer2Dmg) < 0.1)
        {
            _battleLog.Log("Round is a draw", _currentRound);
        }
        else if (cardPlayer1Dmg > cardPlayer2Dmg)
        {
            _deckPlayer2.Remove(cardPlayer2);
            _deckPlayer1.Add(cardPlayer2);
            
            _battleLog.Log($"{_player1.Username} wins round!", _currentRound);
        }
        else if (cardPlayer1Dmg < cardPlayer2Dmg)
        {
            _deckPlayer1.Remove(cardPlayer1);
            _deckPlayer2.Add(cardPlayer1);
            
            _battleLog.Log($"{_player2.Username} wins round!", _currentRound);
        }
    }

    private Card RandomCardFromDeck(List<Card> deck)
    {
        Random random = new();
        return deck[random.Next(deck.Count)];
    }

    private float CalculateCardDamage(Card card, Card opponentCard)
    {
        float damageMultiplier = CalculateDamageMultiplier(card, opponentCard);
        int baseDamage = CalculateBaseDamage(card, opponentCard);

        return baseDamage * damageMultiplier;
    }

    private int CalculateBaseDamage(Card card, Card opponentCard)
    {
        MonsterCatalog cardMonster = MonsterCatalogConverter.ToEnum(card.Name);
        MonsterCatalog opponentCardMonster = MonsterCatalogConverter.ToEnum(opponentCard.Name);
     
        if (card.Type == CardType.Spell)
            if (opponentCardMonster == MonsterCatalog.Kraken)
                return 0;
            else
                return card.Damage;
        
        if (cardMonster == MonsterCatalog.Undefined)
            return 0;

        if (cardMonster == MonsterCatalog.Goblin && opponentCardMonster == MonsterCatalog.Dragon)
            return 0;
        
        if (cardMonster == MonsterCatalog.Ork && opponentCardMonster == MonsterCatalog.Wizzard)
            return 0;

        if (cardMonster == MonsterCatalog.Knight && opponentCard.Type == CardType.Spell &&
            opponentCard.ElementType == ElementType.Water)
            return 0;
        
        if (cardMonster == MonsterCatalog.Dragon && opponentCardMonster == MonsterCatalog.Elf &&
            opponentCard.ElementType == ElementType.Fire)
            return 0;
        
        return card.Damage;
    }
    
    private float CalculateDamageMultiplier(Card card, Card opponentCard)
    {
        if (card.Type == CardType.Undefined)
            return 0f;
        
        if (card.Type != CardType.Spell && opponentCard.Type != CardType.Spell)
            return 1f;
        
        if (card.Type == opponentCard.Type)
            return 1f;

        if (card.ElementType == ElementType.Water && opponentCard.ElementType == ElementType.Fire)
            return 2f;
        if (card.ElementType == ElementType.Fire && opponentCard.ElementType == ElementType.Water)
            return .5f;
        
        if (card.ElementType == ElementType.Fire && opponentCard.ElementType == ElementType.Normal)
            return 2f;
        if (card.ElementType == ElementType.Normal && opponentCard.ElementType == ElementType.Fire)
            return .5f;
        
        if (card.ElementType == ElementType.Normal && opponentCard.ElementType == ElementType.Water)
            return 2f;
        if (card.ElementType == ElementType.Water && opponentCard.ElementType == ElementType.Normal)
            return .5f;

        return 1f;
    }
}