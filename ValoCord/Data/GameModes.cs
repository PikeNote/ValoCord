using System.Text.RegularExpressions;

namespace ValoCord.Data;

public static class GameModes
{
    private static readonly Regex gameModeRegex = new Regex("\\/[A-Za-z0-9]+\\/[A-Za-z0-9]+\\/([A-Za-z0-9]+)\\/");
    
    private static readonly Dictionary<string, string> GameModeMappings = 
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

    public static List<string> GameModeList => new[] { "(None)" }
        .Concat(GameModeMappings.Values)
        .ToList();

    public static String ConvertGameMode(string gameMode, Boolean isCompetetive = false)
    {
        if(isCompetetive) { return "Competitive" ;}
        Match gameModeString = gameModeRegex.Match(gameMode);
        if (gameModeString.Captures.Count > 0)
        {
            if (GameModeMappings.TryGetValue(gameModeString.Groups[1].Value, out var convertedGameMode))
            {
                return convertedGameMode;
            };
        }
        return "Unknown";
    }

    public static List<GameModeSettings> GetDefaultGameModeSettings()
    {
        var gameModeSettings = new List<GameModeSettings>();
        foreach (var value in GameModeMappings.Values)
        {
            gameModeSettings.Add(new GameModeSettings(){ GameMode = value, Enabled = true});
        }
        return gameModeSettings;
    }
}