using ValAPINet;

namespace ValoCord.Data;

public class GameData
{
    public required string Agent { get; set; }
    public required String Map { get; set; }
    public required String MatchId { get; set; }
    public List<long> RoundStartTimeStamps { get; set; } = new List<long>();
    public List<RoundData> RoundEvents { get; set;  } = new List<RoundData>();
    public Dictionary<String, PlayerData> Players { get; set;  } = new Dictionary<String, PlayerData>();
    public String PlayerUuid { get; set; } = "";
    public String PlayerTeam { get; set; } = "";
    public List<MatchData.Team> Teams { get; set; } = new();
    public Boolean IsCompetetive { get; set; }
    public required String Mode { get; set; }
    public string Date { get; set; } = "";
    public int Standing { get; set; }
    public long MatchStartTime { get; set; }
    public long RecordingStartTime { get; set; }
    public void AddRoundTimestamp()
    {
        RoundStartTimeStamps.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds());
    }
}

public class PlayerData
{
    public required string Uuid { get; init; }
    public required string CharacterPlayed { get; init; }
    public required string TeamId { get; init; }
    public required string Username { get; init; }
    public required string Tag { get; init; }
    public int Kills { get; init; }
    public int Deaths { get; init; }
    public int Assists { get; init; }
    public int CombatScore { get; init; }
    public required List<List<MatchData.Damage>> DamageBreakdown { get; init; }
}

public class GameKill : RoundEvent
{
    public float TimeKillIntoGame { get; set; }
    public required String GunUsed { get; set; } 
    public required String AgentKilled { get; set; }
    public required String AgentKilling { get; set; }
}

public class RoundEvent
{
    public RoundEventType EventType { get; set; } 
    public float TimeIntoRound { get; init; }
}

public enum RoundEventType
{
    KillEvent,
    BombPlanted,
    BombDefused
}

public class RoundData
{
    public String TeamWon { get; set; } = "Red";
    public String EndType { get; set; } = "Eliminated";
    public List<RoundEvent> RoundEvents { get; set; } = new();
    public int RoundNumber { get; set; }
    
}

