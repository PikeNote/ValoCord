using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ValoCord_WPF.Data;

public class GameModes
{
    private static readonly Regex gameModeRegex = new Regex("\\/[A-Za-z0-9]+\\/[A-Za-z0-9]+\\/([A-Za-z0-9]+)\\/");
    
    private static readonly Dictionary<string, string> _gameModeMappings = 
        new(StringComparer.OrdinalIgnoreCase)
        {
            {"QuickBomb", "Spike Rush"},
            {"OneForAll", "One For All"},
            {"SnowballFight", "Snowball Fight"},
            {"_Development", "Swift Play"},
            {"HURM", "Team Deathmatch"},
            {"Deathmatch", "Deathmatch"},
            {"Bomb", "Unrated"}
        };

    public static String ConvertGameMode(string gameMode, Boolean isCompetetive = false)
    {
        if(isCompetetive) { return "Competitive" ;}
        Match gameModeString = gameModeRegex.Match(gameMode);
        if (gameModeString.Captures.Count > 0)
        {
            if (_gameModeMappings.TryGetValue(gameModeString.Groups[1].Value, out var convertedGameMode))
            {
                return convertedGameMode;
            };
        }
        return "Unknown";
    }

    public static List<GameModeSettings> GetDefaultGameModeSettings()
    {
        var gameModeSettings = new List<GameModeSettings>();
        foreach (var value in _gameModeMappings.Values)
        {
            gameModeSettings.Add(new GameModeSettings(){ GameMode = value, Enabled = true});
        }
        return gameModeSettings;
    }
}