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
            { "Infinity", "Abyss" },
    
            // TDM Maps
            { "HURM_Alley", "Piazza" },
            { "HURM_Yard", "District" },
            { "HURM_Helix", "Drift" },
            { "HURM_Bowl", "Kasbah" }
        };
    
    private static readonly Dictionary<string, string> _mapAssetMappings = 
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Unrated/Comp Maps
            { "Ascent", "/Assets/Maps/Ascent.png" },
            { "Triad", "/Assets/Maps/Haven.png" },
            { "Duality", "/Assets/Maps/Bind.png" },
            { "Bonsai", "/Assets/Maps/Split.png" },
            { "Port", "/Assets/Maps/Icebox.png" },
            { "Foxtrot", "/Assets/Maps/Breeze.png" },
            { "Canyon", "/Assets/Maps/Fracture.png" },
            { "Pitt", "/Assets/Maps/Pearl.png" },
            { "Jam", "/Assets/Maps/Lotus.png" },
            { "Juliett", "/Assets/Maps/Sunset.png" },
            { "Infinity", "/Assets/Maps/Abyss.png" },
    
            // TDM Maps
            { "HURM_Alley", "/Assets/Maps/Piazza.png" },
            { "HURM_Yard", "/Assets/Maps/District.png" },
            { "HURM_Helix", "/Assets/Maps/Drift.png" },
            { "HURM_Bowl", "/Assets/Maps/Kasbah.png" }
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

    public static string GetFileName(string codeName)
    {
        if (string.IsNullOrEmpty(codeName))
        {
            return "";
        }
        if (_mapAssetMappings.TryGetValue(codeName, out var displayName))
        {
            return displayName;
        }
        return "/Assets/Maps/Default.png";
    }
}