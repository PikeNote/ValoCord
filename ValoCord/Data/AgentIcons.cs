﻿using System;
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
    
    private static readonly Dictionary<string, string> _agentUUIDMappings = 
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "e370fa57-4757-3604-3648-499e1f642d3f", "Gekko" },
            { "dade69b4-4f5a-8528-247b-219e5a1facd6", "Fade" },
            { "5f8d3a7f-467b-97f3-062c-13acf203c006", "Breach" },
            { "cc8b64c8-4b25-4ff9-6e7f-37b4da43d235", "Deadlock" },
            { "b444168c-4e35-8076-db47-ef9bf368f384", "Tejo" },
            { "f94c3b30-42be-e959-889c-5aa313dba261", "Raze" },
            { "22697a3d-45bf-8dd7-4fec-84a9e28c69d7", "Chamber" },
            { "601dbbe7-43ce-be57-2a40-4abd24953621", "KAY/O" },
            { "6f2a04ca-43e0-be17-7f36-b3908627744d", "Skye" },
            { "117ed9e3-49f3-6512-3ccf-0cada7e3823b", "Cypher" },
            { "320b2a48-4d9b-a075-30f1-1f93a9b638fa", "Sova" },
            { "1e58de9c-4950-5125-93e9-a0aee9f98746", "Killjoy" },
            { "95b78ed7-4637-86d9-7e41-71ba8c293152", "Harbor" },
            { "efba5359-4016-a1e5-7626-b1ae76895940", "Vyse" },
            { "707eab51-4836-f488-046a-cda6bf494859", "Viper" },
            { "eb93336a-449b-9c1b-0a54-a891f7921d69", "Phoenix" },
            { "41fb69c1-4189-7b37-f117-bcaf1e96f1bf", "Astra" },
            { "9f0d8ba9-4140-b941-57d3-a7ad57c6b417", "Brimstone" },
            { "0e38b510-41a8-5780-5e8f-568b2a4f2d6c", "Iso" },
            { "1dbf2edd-4729-0984-3115-daa5eed44993", "Clove" },
            { "bb2a4828-46eb-8cd1-e765-15848195d751", "Neon" },
            { "7f94d92c-4234-0a36-9646-3a87eb8b5c89", "Yoru" },
            { "df1cb487-4902-002e-5c17-d28e83e78588", "Waylay" },
            { "569fdd95-4d10-43ab-ca70-79becc718b46", "Sage" },
            { "a3bfb853-43b2-7238-a4f1-ad90e9e46bcc", "Reyna" },
            { "8e253930-4c05-31dd-1b6c-968525494517", "Omen" },
            { "add6443a-41bd-e414-f6ad-e58d267f4e95", "Jett" }
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
    
    public static string GetAgentNames(string uuid)
    {
        if (string.IsNullOrEmpty(uuid))
        {
            return "";
        }
        if (_agentUUIDMappings.TryGetValue(uuid, out var displayName))
        {
            return displayName;
        }

        return "Jett";
    }
}