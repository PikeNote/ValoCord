using System;
using System.Collections.Generic;

namespace ValoCord.Data;

public class AgentIcons
{
    private static readonly Dictionary<string, string> _agentNameMappings = 
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Jett", "/Assets/Agents/Jett.png" },
            { "Astra", "/Assets/Agents/Astra.png" },
            { "Breach", "/Assets/Agents/Breach.png" },
            { "Brimstone", "/Assets/Agents/Brimstone.png" },
            { "Chamber", "/Assets/Agents/Chamber.png" },
            { "Clove", "/Assets/Agents/Clove.png" },
            { "Cypher", "/Assets/Agents/Cypher.png" },
            { "Deadlock", "/Assets/Agents/Deadlock.png" },
            { "Fade", "/Assets/Agents/Fade.png" },
            { "Gekko", "/Assets/Agents/Gekko.png" },
            { "Harbor", "/Assets/Agents/Harbor.png" },
            { "Iso", "/Assets/Agents/Iso.png" },
            { "KAY/O", "/Assets/Agents/KAYO.png" },
            { "Killjoy", "/Assets/Agents/Killjoy.png" },
            { "Neon", "/Assets/Agents/Neon.png" },
            { "Omen", "/Assets/Agents/Omen.png" },
            { "Phoenix", "/Assets/Agents/Phoenix.png" },
            { "Raze", "/Assets/Agents/Raze.png" },
            { "Reyna", "/Assets/Agents/Reyna.png" },
            { "Sage", "/Assets/Agents/Sage.png" },
            { "Skye", "/Assets/Agents/Skye.png" },
            { "Sova", "/Assets/Agents/Sova.png" },
            { "Viper", "/Assets/Agents/Viper.png" },
            { "Yoru", "/Assets/Agents/Yoru.png" },
            { "Waylay", "/Assets/Agents/Waylay.png" },

        };

    public static string GetAgentIcons(string codeName)
    {
        if (string.IsNullOrEmpty(codeName))
        {
            return "";
        }
        if (_agentNameMappings.TryGetValue(codeName, out var displayName))
        {
            return displayName;
        }
        return _agentNameMappings["Jett"];
    }
}