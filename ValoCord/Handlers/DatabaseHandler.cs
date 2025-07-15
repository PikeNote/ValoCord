using LiteDB;
using ValAPINet;
using ValoCord.Data;

namespace ValoCord.Handlers;

public static class DatabaseHandler
{
    public static LiteDatabase MainDatabase = new LiteDatabase($"Filename={Paths.DefaultDatabasePath};Connection=shared");
    public static ILiteCollection<GameData> GameCollection = MainDatabase.GetCollection<GameData>("games");

    public static void Initialize()
    {
        BsonMapper.Global.Entity<MatchData.Team>()
            .Field(x => x.teamId, "team_id"); 
    }
    public static void InsertGame(GameData gd)
    {
        GameCollection.Insert(gd);
    }

    public static List<GameData> GetRecentGames()
    {
        var results = GameCollection.Query()
            .OrderByDescending(x => x.recordingStartTime)
            .Limit(6)
            .ToList();
        return results;
    }
}