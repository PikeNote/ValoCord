using System;
using System.Collections.Generic;

namespace ValoCord.Data;

public class MapList
{
    private static readonly Dictionary<string, string> _mapNameMappings = 
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Unrated/Comp Maps
            { "Ascent", "Ascent" },
            { "Triad", "Haven" },
            { "Duality", "Bind" },
            { "Bonsai", "Split" },
            { "Port", "Icebox" },
            { "Foxtrot", "Breeze" },
            { "Canyon", "Fracture" },
            { "Pitt", "Pearl" },
            { "Jam", "Lotus" },
            { "Juliett", "Sunset" },
            { "Infinity", "The Range" },
    
            // TDM Maps
            { "HURM_Alley", "Piazza" },
            { "HURM_Yard", "District" },
            { "HURM_Helix", "Drift" }
        };

    public static string GetDisplayName(string codeName)
    {
        if (string.IsNullOrEmpty(codeName))
        {
            return "Unknown Map";
        }
        if (_mapNameMappings.TryGetValue(codeName, out var displayName))
        {
            return displayName;
        }
        return codeName;
    }
}