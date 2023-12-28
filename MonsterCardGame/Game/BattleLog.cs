namespace MonsterCardGame.Game;

public class BattleLog
{
    private List<string> _log = new();

    public void Log(string message, int roundNumber)
    {
        _log.Add($"[Round {roundNumber}] {message}");
    }

    public List<string> GetList()
    {
        return _log;
    }
}