using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ValAPINet;
using ValoCord.Data;
using ValoCord.Extentions;
using ValoCord.Handlers;

namespace ValoCord.ViewModels;

public class VODListItemViewModel : ViewModelBase, INotifyPropertyChanged
{
    private readonly GameData _gameData;

    public string MapName => $"{MapList.GetDisplayName(_gameData.map)}";
    public string GameMode => _gameData.mode;
    public string Date => _gameData.date;
    public string Agent => AgentIcons.GetAgentNames(_gameData.agent);
    public string Standing => _gameData.standing.ToOrdinal();
    public Bitmap AgentIcon => LoadFromResource(new Uri($"avares://Valocord{AgentIcons.GetAgentIcons(Agent)}"));
    public Bitmap MapImage => LoadFromResource(new Uri($"avares://Valocord{MapList.GetFileName(_gameData.map)}"));

    public static Bitmap LoadFromResource(Uri resourceUri)
    {
        return new Bitmap(AssetLoader.Open(resourceUri));
    }
    
    public String CombatScore
    {
        get
        {
            return $"{_gameData._players.First(player => player.uuid == _gameData.playerUUID).combat_score/_gameData.teams.First().roundsPlayed} ACS";
        }
    }
    
    public MatchData.Damage Damage {
        get
        {
            var damage = new MatchData.Damage();
            var damageList = _gameData._players.First(player => player.uuid == _gameData.playerUUID).damage_breakdown;
            foreach (var roundDamages in damageList)
            {
                foreach (var damageGiven in roundDamages)
                {
                    damage.headshots += damageGiven.headshots;
                    damage.bodyshots += damageGiven.bodyshots;
                    damage.legshots += damageGiven.legshots;
                }
            }

            return damage;
        }
    }

    public int TotalShots => Damage.headshots + Damage.bodyshots + Damage.legshots;

    public String HeadPecentage
    {
        get
        {
            if (TotalShots == 0)
            {
                return "0.0% (0)";
            }
            
            double percentage = (double)Damage.headshots / TotalShots * 100;
            return $"{percentage:F1}% ({Damage.headshots})";
        }
    }
    public String BodyPercentage     
    {
        get
        {
            if (TotalShots == 0)
            {
                return "0.0% (0)";
            }
            
            double percentage = (double)Damage.bodyshots / TotalShots * 100;
            return $"{percentage:F1}% ({Damage.bodyshots})";
        }
    }
    public String LegPercentage
    {
        get
        {
            if (TotalShots == 0)
            {
                return "0.0% (0)";
            }
            
            double percentage = (double)Damage.legshots / TotalShots * 100;
            return $"{percentage:F1}% ({Damage.legshots})";
        }
    }
    
    

    public string Score
    {
        get
        {
            var playerScore = _gameData.teams.First(team => team.teamId == _gameData.playerTeam);
            var nonPlayerScore = _gameData.teams.First(team => team.teamId != _gameData.playerTeam);
            return $"{playerScore.roundsWon}-{nonPlayerScore.roundsWon}";
        }
    }
    
    public VODListItemViewModel() { }
    
    public VODListItemViewModel(GameData gameData)
    {
        _gameData = gameData;
        Console.WriteLine(AgentIcons.GetAgentIcons(Agent));
    }

    public GameData GetGameData()
    {
        return _gameData;
    }

}