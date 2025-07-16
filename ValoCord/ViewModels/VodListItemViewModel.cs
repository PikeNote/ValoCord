using System.ComponentModel;
using System.Windows.Media.Imaging;
using ValAPINet;
using ValoCord.Data;
using ValoCord.Extentions;

namespace ValoCord.ViewModels;

public class VodListItemViewModel : ViewModelBase
{
    public required GameData GameData { get; init; }

    public string MapName => $"{MapData.GetDisplayName(GameData.Map)}";
    public string GameMode => GameModes.ConvertGameMode(GameData.Mode);
    public string Date => GameData.Date;
    public string Agent => AgentData.GetAgentNames(GameData.Agent);
    public string Standing => GameData.Standing.ToOrdinal();
    public BitmapImage AgentIcon => LoadFromResource(new Uri($"pack://application:,,,{AgentData.GetAgentIcons(Agent)}"));
    public BitmapImage MapImage => LoadFromResource(new Uri($"pack://application:,,,{MapData.GetFileName(GameData.Map)}"));
    public long RecordingStartTime => GameData.RecordingStartTime;
    public string TeamWon
    {
        get
        {
            if (GameData.Teams[0].roundsWon == GameData.Teams[1].roundsWon)
                return "Draw";
            var teamWon = GameData.Teams[0].roundsWon > GameData.Teams[1].roundsWon ? GameData.Teams[0].teamId : GameData.Teams[1].teamId;
            if (teamWon == GameData.PlayerTeam) 
                return "Won";
            return "Lost";
        }
    }

    private static BitmapImage LoadFromResource(Uri resourceUri)
    {
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.DecodePixelWidth = 228;
        bitmapImage.UriSource = resourceUri;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return bitmapImage;
    }

    public string Kda => $"{GameData.Players[GameData.PlayerUuid].Kills}/{GameData.Players[GameData.PlayerUuid].Deaths}/{GameData.Players[GameData.PlayerUuid].Assists}";
    
    public String CombatScore => $"{GameData.Players[GameData.PlayerUuid].CombatScore/GameData.Teams.First().roundsPlayed} ACS";

    private MatchData.Damage Damage {
        get
        {
            var damage = new MatchData.Damage();
            var damageList = GameData.Players[GameData.PlayerUuid].DamageBreakdown;
            foreach (var damageGiven in damageList.SelectMany(roundDamages => roundDamages))
            {
                damage.headshots += damageGiven.headshots;
                damage.bodyshots += damageGiven.bodyshots;
                damage.legshots += damageGiven.legshots;
            }

            return damage;
        }
    }

    private int TotalShots => Damage.headshots + Damage.bodyshots + Damage.legshots;

    public string HeadPecentage
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
    public string BodyPercentage     
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
    public string LegPercentage
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
            var playerScore = GameData.Teams.First(team => team.teamId == GameData.PlayerTeam);
            var nonPlayerScore = GameData.Teams.First(team => team.teamId != GameData.PlayerTeam);
            return $"{playerScore.roundsWon}-{nonPlayerScore.roundsWon}";
        }
    }
    
    public GameData GetGameData()
    {
        return GameData;
    }

}