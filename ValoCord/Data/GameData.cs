using System;
using System.Collections.Generic;
using ValAPINet;

namespace ValoCord.Data;

public class GameData
{
    public GameData() {}
    public String agent { get; set; }
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
    public PlayerData() { }
    public string uuid { get; set; }
    public string character_played { get; set; }
    public string team_id { get; set; }
    public string username { get; set; }
    public string tag { get; set; }
    public int kills { get; set; }
    public int deaths { get; set; }
    public int assists { get; set; }
    public int combat_score { get; set; }
    public List<List<MatchData.Damage>> damage_breakdown { get; set; }
}

public class GameKill
{
    public float TimeKillIntoRound { get; set; }
    public float TimeKillIntoGame { get; set; }
    public String GunUsed { get; set; } 
    public String AgentKilled { get; set; }
    public String AgentKilling { get; set; }

    public GameKill() { }
}

public class RoundData
{
    public RoundData() { }
    public String TeamWon { get; set; }
    public String EndType { get; set; }
    public int BombPlantTime { get; set; }
    public int BombDefuseTime { get; set; }
    public List<GameKill> KillEvents { get; set; }
    public int RoundNumber { get; set; }
    
}

