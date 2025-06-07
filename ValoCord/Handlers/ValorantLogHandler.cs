using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;
using Newtonsoft.Json;
using NLog;
using ScreenRecorderLib;
using Svg;
using ValAPINet;

namespace ValoCord.Handlers;

public static class ValorantLogHandler
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static FileStream _activeFileStream;
    private static StreamReader _activeStreamReader;
    private static readonly String path = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData), "VALORANT\\Saved\\Logs\\ShooterGame.log");
    private static readonly String MapLoadRegex =
        @"\[Map Name: ([A-Za-z0-9]+) \| Changed: [A-Za-z0-9]+\] \[Local World: [A-Za-z0-9]+ \| Changed: [A-Za-z0-9]+\] \[Match Setup: [A-Za-z0-9]+ \| Changed: [A-Za-z0-9]+\] \[Map Ready: ([A-Za-z0-9]+) \| Changed: [A-Za-z0-9]+\] \[Map Complete: ([A-Za-z0-9]+) \| Changed: [A-Za-z0-9]+\] \[URL: (?:(?:25[0-5]|(?:2[0-4]|1\d|[1-9]|)\d)\.?\b){4}:(?:[+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE](?:[+-]?\d+))?\/Game\/Maps\/[A-Za-z0-9]+\/[A-Za-z0-9]+\?Name=[\p{L}\p{N} ]+\?SubjectBase64=[A-Za-z0-9]+\?game=\/Game\/GameModes\/[\/_A-Za-z0-9]+\/([_A-Za-z0-9]+(?:\.[_A-Za-z0-9]+)+)_C#([a-zA-Z]+)]";
    private static readonly String RoundStartRegex =
        @"Warning: Gameplay started at local time [0-9]*\.[0-9]+ \(server time ([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE]([+-]?\d+))?\)";
    private static readonly String MainMenuRegex =
        @"\[Map Name: ([A-Za-z0-9]+) \| Changed: [A-Za-z0-9]+\] \[Local World: [A-Za-z0-9]+ \| Changed: [A-Za-z0-9]+\] \[Match Setup: [A-Za-z0-9]+ \| Changed: [A-Za-z0-9]+\] \[Map Ready: ([A-Za-z0-9]+) \| Changed: [A-Za-z0-9]+\] \[Map Complete: ([A-Za-z0-9]+) \| Changed: [A-Za-z0-9]+\] \[URL: \/Game\/Maps\/[A-Za-z0-9]+\/[A-Za-z0-9]+\?Name=[\p{L}\p{N} ]+\?SubjectBase64=[A-Za-z0-9#]+]";

    private static readonly String[] PlayableMaps =
        { "Ascent", "Triad", "Duality", "Bonsai", "Port", "Foxtrot", "Canyon", "Pitt", "Infinity", "Juliett", "Jam", "HURM_Alley", "HURM_Yard", "HURM_Bowl", "HURM_Helix" };

    private static GameData? _gameData = null;
    static readonly Logger _logger = LogManager.GetLogger("Log Handler");
    

    static ValorantLogHandler()
    {
        
    }
    
    public async static void StartLogging()
    {
        
        await Task.Delay(3000);
        if (File.Exists(path))
        {
            _activeFileStream = File.Open(path, FileMode.Open,
                FileAccess.Read, FileShare.ReadWrite);

            _activeStreamReader = new StreamReader(_activeFileStream);
            
            _activeStreamReader.ReadToEnd();  // Get old stuff out

            String line;
            while (true)  // <= Check for end of file
            {
                if (!_activeStreamReader.EndOfStream)
                {
                    line = Regex.Replace(_activeStreamReader.ReadLine(), @"\t|\r", "");
                    ;
                    ProcessLogging(line);
                }
                
            }
            /*
            for (;;)
            {
                // Sleep for 0.5 seconds before trying to read the logs again
                Thread.Sleep(TimeSpan.FromSeconds(0.5));

                String endMessage = Regex.Replace(_activeStreamReader.ReadToEnd(), @"\t|\r", ""); // Clean up inputs through regex (remove any new lines)

                String[] endMessageList = endMessage.Split("\n");
                
                foreach (var s in endMessageList)
                {
                    if (s != "" && s != "\n")
                    {
                        
                    
                    }
                }
                
                
            }
            */
        }
        
    }

    private static void ProcessLogging(String input)
    {
        LogData ld = new LogData(input);
        switch (ld.ServiceCaller)
        {
            case "LogMapLoadModel": // Map loaded
                
                Match mapLine = Regex.Match(ld.Msg, MapLoadRegex);
                System.Diagnostics.Debug.WriteLine(ld.Msg);
                if (mapLine.Captures.Count > 0 && mapLine.Groups.Count >= 6 && mapLine.Groups[2].Value == "TRUE" && PlayableMaps.Contains(mapLine.Groups[1].Value))
                {
                    if (_gameData == null && mapLine.Groups[3].Value != "TRUE")
                    {
                        //https://glz-ap-1.ap.a.pvp.net/core-game/v1/players/{$playerId}
                        String mid = ValorantAPI.GetCoreMatchID();
                        
                        if (string.IsNullOrWhiteSpace(mid))
                        {
                            _logger.Info("Mid match ID invalid");
                            mid = ValorantAPI.GetPreMatchID();
                            _logger.Info("Prematch ID fetching: " + mid);
                        } // Attempt to get both pre and core to see which one can be gotten
                        _logger.Info("Match ID Fetched: " + mid);
                        if (!string.IsNullOrWhiteSpace(mid))
                        {
                            _gameData = new GameData(mapLine.Groups[1].Value, mapLine.Groups[5].Value, mid);
                            ValorantRecorder.StartRecording(mid);
                        }
                    }
                    else
                    {
                        _logger.Info("Game ended");
                        if (mapLine.Groups[3].Value == "TRUE")
                        {
                            _gameData.MapComplete();
                        }
                        else
                        {
                            if (_gameData != null)
                            {
                                _logger.Info("Skip recording");
                                _logger.Info(input);
                                // Skip writing if the game data was ever initialized at all in the first place
                                
                                ValorantRecorder.StopRecording();
                                _gameData.SkipWritingData();
                            }
                            // how did we get here? 
                            // if game started and is marked as so- this has to be a leave game, dc
                            // dump recording for now amd skip writing :derp:
                        }

                    }
                }
                else
                {

                    if (_gameData != null)
                    {
                        Match lobbyCheck = Regex.Match(ld.Msg, MainMenuRegex);
                        System.Diagnostics.Debug.WriteLine(lobbyCheck.Groups.Count);
                        System.Threading.Thread.Sleep(5000);
                        if (lobbyCheck.Captures.Count > 0 && lobbyCheck.Groups.Count >= 4 &&
                            lobbyCheck.Groups[1].Value.Contains("MainMenu"))
                        {
                            ValorantRecorder.StopRecording();
                            _gameData.RetrieveMatchData();
                            _gameData.WriteToJson();
                        }
                    }
                }
                break;
            case "LogShooterGameState": // Buy round/round
                if (_gameData != null)
                {
                    Match rdStart = Regex.Match(ld.Msg, RoundStartRegex);
                    if (rdStart.Captures.Count > 0 && rdStart.Groups.Count >= 1)
                    {
                        if (float.Parse(rdStart.Groups[1].Value)> 1)
                        {
                            _gameData.AddRoundTimestamp();
                            _logger.Info("Round started");
                        }
                    }
                }
                break;
        }
        
        
        
    }

    public static void StopLogging()
    {
        _activeStreamReader.Close();
        _activeFileStream.Dispose();
        
        // Null the readers just in case
        _activeFileStream = null;
        _activeStreamReader = null;
    }

    public static void ResetGd()
    {
        _gameData = null;
    }
}

class LogData
{
    private const String LogRegex = @"\[([^\]]*)]\[([^\]]*)]([a-zA-Z]+):([A-Za-z0-9 _].+)";
    public DateTime Dt { get; set; }
    public int EventType { get; set; }
    public String ServiceCaller { get; set; } 
    public String Msg { get; set; }

    public LogData(String inputString)
    {
        Match logLine = Regex.Match(inputString, LogRegex); // [2022.11.19-06.34.51:565][982]LogUMGSequenceTickManager: Warning: User widget GenericTooltipWrapper is playing a looping animation while invisible/collapsed.

        if (!(logLine.Captures.Count > 0 && logLine.Groups.Count >= 4))
        {
            return;
        }

        Dt = DateTime.ParseExact(logLine.Groups[1].Value,"yyyy.MM.dd-HH.mm.ss:fff",null); // 2022.11.19-06.34.54:081
        EventType = Int32.Parse(logLine.Groups[2].Value);
        ServiceCaller = logLine.Groups[3].Value;
        Msg = logLine.Groups[4].Value;
    }
}

class GameData
{
    public String agent;
    public String map;
    public String matchId;
    public List<String> _roundStartTimeStamps = new List<string>();
    public List<RoundData> _roundEvents = new List<RoundData>();
    public List<PlayerData> _players = new List<PlayerData>();
    public String playerUUID;
    public String playerTeam;
    public List<MatchData.Team> teams = new List<MatchData.Team>();
    
    static Logger logger = LogManager.GetLogger("Game Data");
    private bool skipWriting = false;
    private bool mapCompleted = false;

    public GameData(String map, String agent, String matchId)
    {
        // Basic match information initalized
        this.map = map;
        this.agent = agent;
        this.matchId = matchId;
        this.playerUUID = ValorantAPI.getCurrentUser();
    }

    public void RetrieveMatchData() // Should be only called at the end of game as designated by log
    {
        System.Diagnostics.Debug.WriteLine(matchId);
        MatchData md = ValorantAPI.GetMatchData(matchId);
        logger.Info("MD Status:" + md.StatusCode);
        logger.Info("Match Data:" + matchId); 
        
        if (md != null & md.StatusCode == 200)
        {
            teams = md.teams;
            this.playerTeam = md.players.First(user => user.subject == playerUUID).teamId; 
            System.Diagnostics.Debug.WriteLine(md.StatusCode);
            foreach (var userInfo in md.players)
            {
                List<List<MatchData.Damage>> damages = new List<List<MatchData.Damage>>();
                foreach (var mdRoundResult in md.roundResults)
                {
                    
                    var currentPlayerStat = mdRoundResult.playerStats.FirstOrDefault(user => user.subject == userInfo.subject);
                    if (currentPlayerStat != null)
                    {
                        damages.Add(currentPlayerStat.damage);
                    }
                    
                }

                _players.Add(new PlayerData()
                {
                    uuid = userInfo.subject,
                    character_played = userInfo.characterId,
                    team_id = userInfo.teamId,
                    username = userInfo.gameName,
                    tag = userInfo.tagLine,
                    kills = userInfo.stats.kills,
                    deaths = userInfo.stats.deaths,
                    assists = userInfo.stats.assists,
                    combat_score = userInfo.stats.score,
                    damage_breakdown = damages
                });
            }
            
            foreach (var mdRoundResult in md.roundResults)
            {
                List<GameKill> gk = new List<GameKill>();
                foreach (var playerStat in mdRoundResult.playerStats)
                {
                    if (playerStat.kills.Count > 0)
                    {
                        foreach (var playerStatKill in playerStat.kills)
                        { 
                            gk.Add(new GameKill(playerStatKill.roundTime,playerStatKill.gameTime, playerStatKill.finishingDamage.damageItem, playerStatKill.victim, playerStatKill.killer));
                        }
                    }
                }
                gk.Sort((x,y)=>x.TimeKillIntoRound.CompareTo(y.TimeKillIntoRound));

                _roundEvents.Add(new RoundData(mdRoundResult.winningTeam, mdRoundResult.roundResult,
                    mdRoundResult.defuseRoundTime, mdRoundResult.plantRoundTime, gk));
            }
            logger.Info("Match data processed");
            
            
        }
        else
        {
            skipWriting = true;
        }
        // Process events into:
        // A list of rounds with Losses/Wins, Kills that happened (also time?)
    }

    public void WriteToJson()
    {
        System.Diagnostics.Debug.WriteLine(skipWriting);
        if (!skipWriting)
        {
            System.Diagnostics.Debug.WriteLine("Writing to JSON");
            // Implement JSON initalizer sometime down the line :derp: \
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ValoCord", "data", matchId + ".json"), JsonConvert.SerializeObject(this));
        }
        
        ValorantLogHandler.ResetGd();
    }

    
    
    public void AddRoundTimestamp()
    {
        _roundStartTimeStamps.Add(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
    }

    public void SkipWritingData()
    {
        System.Diagnostics.Debug.WriteLine("???");
        skipWriting = true;
    }

    public void MapComplete()
    {
        mapCompleted = true;
    }
}

class PlayerData
{
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

class GameKill
{
    public float TimeKillIntoRound { get; set; }
    public float TimeKillIntoGame { get; set; }
    public String GunUsed { get; set; } 
    public String AgentKilled { get; set; }
    public String AgentKilling { get; set; }

    public GameKill(float timeKillIntoRound, float timeKillIntoGame, String gunUsed, String agentKilled, String agentKilling)
    {
        this.TimeKillIntoRound = timeKillIntoRound;
        this.TimeKillIntoGame = timeKillIntoGame;
        this.GunUsed = gunUsed;
        this.AgentKilled = agentKilled;
        this.AgentKilling = agentKilling;
    }
}

class RoundData
{
    public String TeamWon { get; set; }
    public String EndType { get; set; }
    public float? BombPlantTime { get; set; }
    public float? BombDefuseTime { get; set; }
    public List<GameKill> KillEvents { get; set; }

    public RoundData(String teamWon, String endType, float? bombDefuseTime, float? bombPlantTime, List<GameKill> killEvents)
    {
        if (bombDefuseTime == null)
        {
            bombDefuseTime = 0;
        }

        if (bombPlantTime == null)
        {
            bombPlantTime = 0;
        }

        this.TeamWon = teamWon;
        this.EndType = endType;
        this.BombPlantTime = bombPlantTime;
        this.BombDefuseTime = bombDefuseTime;
        this.KillEvents = killEvents;
    }  
}