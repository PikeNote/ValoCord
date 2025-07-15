using System;
using System.Collections.Generic;
using SharpVectors.Dom.Events;
using ValAPINet;

namespace ValoCord.Data;

public class GameData
{
    public GameData() {}
    public string agent { get; set; }
    public String map { get; set; }
    public String matchId { get; set; }
    public List<long> _roundStartTimeStamps { get; set; } = new List<long>();
    public List<RoundData> _roundEvents { get; set;  } = new List<RoundData>();
    public Dictionary<String, PlayerData> _players { get; set;  } = new Dictionary<String, PlayerData>();
    public String playerUUID { get; set; }
    public String playerTeam { get; set; }
    public List<MatchData.Team> teams { get; set; }
    public Boolean isCompetetive { get; set; }
    public String mode { get; set; }
    public string date  { get; set; }
    public int standing { get; set; }
    public long matchStartTime { get; set; }
    public long recordingStartTime { get; set; }
    public void AddRoundTimestamp()
    {
        _roundStartTimeStamps.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds());
    }
}

public class PlayerData
{
    public required string uuid { get; set; }
    public required string character_played { get; set; }
    public required string team_id { get; set; }
    public required string username { get; set; }
    public required string tag { get; set; }
    public int kills { get; set; }
    public int deaths { get; set; }
    public int assists { get; set; }
    public int combat_score { get; set; }
    public required List<List<MatchData.Damage>> damage_breakdown { get; set; }
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
    public float TimeIntoRound { get; set; }
}

public enum RoundEventType
{
    KillEvent,
    BombPlanted,
    BombDefused
}

public class RoundData
{
    public RoundData() { }
    public String TeamWon { get; set; }
    public String EndType { get; set; }
    public List<RoundEvent> RoundEvents { get; set; }
    public int RoundNumber { get; set; }
    
}

