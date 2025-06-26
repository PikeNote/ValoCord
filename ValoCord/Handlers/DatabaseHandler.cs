using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiteDB;
using ValoCord.Data;

namespace ValoCord.Handlers;

public static class DatabaseHandler
{
    public static LiteDatabase MainDatabase = new LiteDatabase(Paths.DefaultDatabasePath);
    public static ILiteCollection<GameData> GameCollection = MainDatabase.GetCollection<GameData>("games");

    public static void InsertGame(GameData gd)
    {
        GameCollection.Insert(gd);
    }

    public static List<GameData> GetRecentGames()
    {
        var results = GameCollection.Query()
            .OrderBy(x => x.recordingStartTime)
            .Limit(6)
            .ToList();
        return results;
    }
}