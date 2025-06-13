using System;
using System.Collections.Generic;

namespace ValoCord.Data;

public static class WeaponData
{
    private static readonly Dictionary<string, string> _weaponUUIDMappings = 
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "29a0cfab-485b-f5d5-779a-b59f85e204a8", "Classic" },
            { "f7e1b454-4ad4-1063-ec0a-159e56b58941", "Stinger"},
            { "462080d1-4035-2937-7c09-27aa2a5c27a7", "Spectre" },
            { "1baa85b4-4c70-1284-64bb-6481dfc3bb4e", "Ghost" },
            { "e336c6b8-418d-9340-d77f-7a9e4cfe0702", "Sheriff" },
            { "c4883e50-4494-202c-3ec3-6b8a9284f00b", "Marshal" },
            { "ae3de142-4d85-2547-dd26-4e90bed35cf7", "Bulldog" },
            { "a03b24d3-4319-996d-0f8c-94bbfba1dfc7", "Operator" },
            { "ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a", "Phantom" },
            { "910be174-449b-c412-ab22-d0873436b21b", "Bucky" },
            { "44d4e95c-4157-0037-81b2-17841bf2e8e3", "Frenzy" },
            { "42da8ccc-40d5-affc-beec-15aa47b42eda", "Shorty" },
            { "55d8a0f4-4274-ca67-fe2c-06ab45efdf58", "Ares" },
            { "9c82e19d-4575-0200-1a81-3eacf00cf872", "Vandal" },
            { "63e6c2b6-4a8e-869c-3d4c-e38355226584", "Odin" },
            { "5f0aaf7a-4289-3998-d5ff-eb9a5cf7ef5c", "Outlaw" },
            { "ec845bf4-4f79-ddda-a3da-0db3774b2794", "Judge" },
            { "4ade7faa-4cf1-8376-95ef-39884480959b", "Guardian" },
            { "2f59173c-4bed-b6c3-2191-dea9b58be9c7", "Melee" },
            { "3de32920-4a8f-0499-7740-648a5bf95470", "Golden Gun" },
            { "0afb2636-4093-c63b-4ef1-1e97966e2a3e", "Spike"},
            { "856D9A7E-4B06-DC37-15DC-9D809C37CB90", "Headhunter"},
            { "39099FB5-4293-DEF4-1E09-2E9080CE7456", "Tour de Force"},
            { "95336AE4-45D4-1032-CFAF-6BAD01910607", "Overdrive"}
        };
    
    private static readonly Dictionary<string, string> _weaponImageMappings = 
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Classic", "/Assets/KillFeed/Classic.png" },
            { "Spectre", "/Assets/KillFeed/Spectre.png" },
            { "Ghost", "/Assets/KillFeed/Ghost.png" },
            { "Frenzy", "/Assets/KillFeed/Frenzy.png" },
            { "Vandal", "/Assets/KillFeed/Vandal.png" },
            { "Phantom", "/Assets/KillFeed/Phantom.png" },
            { "Guardian", "/Assets/KillFeed/Guardian.png" },
            { "Bulldog", "/Assets/KillFeed/Bulldog.png" },
            { "Sheriff", "/Assets/KillFeed/Sheriff.png" },
            { "Stinger", "/Assets/KillFeed/Stinger.png" },
            { "Shorty", "/Assets/KillFeed/Shorty.png" },
            { "Bucky", "/Assets/KillFeed/Bucky.png" },
            { "Judge", "/Assets/KillFeed/Judge.png" },
            { "Odin", "/Assets/KillFeed/Odin.png" },
            { "Ares", "/Assets/KillFeed/Ares.png" },
            { "Marshal", "/Assets/KillFeed/Marshal.png" },
            { "Operator", "/Assets/KillFeed/Operator.png" },
            { "Melee", "/Assets/KillFeed/Melee.png" },
            { "Golden Gun", "/Assets/KillFeed/Golden_Gun.png" },
            { "Spike", "/Assets/KillFeed/Spike.png"},
            { "Headhunter", "/Assets/KillFeed/Headhunter.png"},
            { "Tour de Force", "/Assets/KillFeed/TourDeForce.png"},
            { "Overdrive", "/Assets/KillFeed/Overdrive.png"}
        };
    
    public static string GetDisplayName(string codeName)
    {
        if (string.IsNullOrEmpty(codeName))
        {
            return "Unknown Weapon";
        }
        if (_weaponUUIDMappings.TryGetValue(codeName, out var displayName))
        {
            Console.WriteLine(displayName);
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
        if (_weaponImageMappings.TryGetValue(codeName, out var displayName))
        {
            return displayName;
        }
        return "/Assets/KillFeed/Classic.png";
    }
}