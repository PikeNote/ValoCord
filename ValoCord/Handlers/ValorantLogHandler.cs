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
using ValoCord.Data;

namespace ValoCord.Handlers;

public static class ValorantLogHandler
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static FileStream _activeFileStream;
    private static StreamReader _activeStreamReader;
    private static readonly String path = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData), "VALORANT\\Saved\\Logs\\ShooterGame.log");
    private static readonly String MapLoadRegex =
        @"\[Map Name: ([_A-Za-z0-9]+) \| Changed: [A-Za-z0-9]+\] \[Local World: [A-Za-z0-9]+ \| Changed: [A-Za-z0-9]+\] \[Match Setup: [A-Za-z0-9]+ \| Changed: [A-Za-z0-9]+\] \[Map Ready: ([A-Za-z0-9]+) \| Changed: [A-Za-z0-9]+\] \[Map Complete: ([A-Za-z0-9]+) \| Changed: [A-Za-z0-9]+\] \[URL: (?:(?:25[0-5]|(?:2[0-4]|1\d|[1-9]|)\d)\.?\b){4}:(?:[+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE](?:[+-]?\d+))?\/Game\/Maps\/[A-Za-z0-9]+\/[\/_A-Za-z0-9]+\?Name=[\p{L}\p{N} ]+\?SubjectBase64=[A-Za-z0-9]+\?game=\/Game\/GameModes\/[\/_A-Za-z0-9]+\/([_A-Za-z0-9]+(?:\.[_A-Za-z0-9]+)+)_C#([a-zA-Z]+)]";
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
                if (mapLine.Captures.Count > 0 && mapLine.Groups.Count >= 6 && 
                    mapLine.Groups[2].Value == "TRUE" && PlayableMaps.Contains(mapLine.Groups[1].Value))
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
                            _gameData = new GameData()
                            {
                                map = mapLine.Groups[1].Value,
                                agent = mapLine.Groups[5].Value,
                                matchId = mid
                            };
                            ValorantRecorder.StartRecording(mid);
                        }
                    }
                    else
                    {
                        if (_gameData != null)
                        {
                            break;
                        }

                        _logger.Info("Game ended");
                        if (mapLine.Groups[3].Value == "TRUE")
                        {
                           
                        }
                        else
                        {
                            _logger.Info("Skip recording");
                            _logger.Info(input);
                            // Skip writing if the game data was ever initialized at all in the first place
                            
                            ValorantRecorder.StopRecording();
                        
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
                        System.Threading.Thread.Sleep(1000);
                        if (lobbyCheck.Captures.Count > 0 && lobbyCheck.Groups.Count >= 4 &&
                            lobbyCheck.Groups[1].Value.Contains("MainMenu"))
                        {
                            ValorantRecorder.StopRecording();
                            var matchDataRetrievalSuccess = _gameData.RetrieveMatchData();
                            if (matchDataRetrievalSuccess)
                            {
                                _gameData.WriteToJson();
                            }
                            
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
    
    public static Boolean RetrieveMatchData(this GameData gd) // Should be only called at the end of game as designated by log
    {
        Logger logger = LogManager.GetLogger("Match Data");
        gd.playerUUID = ValorantAPI.getCurrentUser();
        System.Diagnostics.Debug.WriteLine(gd.matchId);
        MatchData md = ValorantAPI.GetMatchData(gd.matchId);
        logger.Info("MD Status:" + md.StatusCode);
        logger.Info("Match Data:" + gd.matchId); 
        
        if (md != null & md.StatusCode == 200)
        {
            MatchData.Player currentPlayer = md.players.First(user => user.subject == gd.playerUUID);
            gd.date = DateTime.Today.ToString("MM/dd/yyyy");
            gd.mode = md.matchInfo.gameMode;
            gd.teams = md.teams;
            gd.playerTeam = currentPlayer.teamId;
            gd.startTime = md.matchInfo.gameStartMillis;
            gd.agent = currentPlayer.characterId;
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

                gd._players.Add(new PlayerData()
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
                
                gd.standing = gd._players
                    .OrderByDescending(player => player.combat_score)
                    .ToList()
                    .FindIndex(p => p.uuid == gd.playerUUID) + 1;

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

                gd._roundEvents.Add(new RoundData()
                    {
                        BombDefuseTime = mdRoundResult.defuseRoundTime,
                        BombPlantTime = mdRoundResult.plantRoundTime,
                        EndType = mdRoundResult.roundResult,
                        KillEvents = gk,
                        RoundNumber = mdRoundResult.roundNum,
                        TeamWon = mdRoundResult.winningTeam
                    });
            }
            logger.Info("Match data processed");
            return true;
            
        }
        else
        {
            return false;
        }
        // Process events into:
        // A list of rounds with Losses/Wins, Kills that happened (also time?)
    }
    
    public static void WriteToJson(this GameData gd)
    {
        System.Diagnostics.Debug.WriteLine("Writing to JSON");
        // Implement JSON initalizer sometime down the line :derp: \
        File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ValoCord", "data", gd.matchId + ".json"), JsonConvert.SerializeObject(gd));
        ResetGd();
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