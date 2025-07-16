using NLog;

namespace ValoCord.Handlers;
using ValAPINet;
using Newtonsoft.Json;
using RestSharp;
using System;

public abstract class ValorantApi
{
    private static Auth? _localAuth;
    private static readonly Logger Logger = LogManager.GetLogger("Valorant API");

    public static Task<Auth> ReAuthAttempt()
    {
        _localAuth = Websocket.GetAuthLocal();
        Logger.Info("User Authenticated");
        return Task.FromResult(_localAuth);
    }
    
    public static void ResetAuth()
    {
        _localAuth = null;
    }

    public static String GetCurrentUser()
    {
        return _localAuth?.subject ?? "";
    }

    public static Boolean CheckAuth()
    {
        return _localAuth == null;
    }

    public static MatchData? GetMatchData(String matchId)
    {
        //return MatchData.GetMatchData(localAuth, matchID);
        if (_localAuth != null)
        {
            RestClient obj = new RestClient("https://pd." + _localAuth.region + ".a.pvp.net/match-details/v1/matches/" + matchId) {
                CookieContainer = _localAuth.cookies
            };
            RestRequest restRequest = new RestRequest(Method.GET);
            restRequest.AddHeader("Authorization", "Bearer " + _localAuth.AccessToken);
            restRequest.AddHeader("X-Riot-Entitlements-JWT", _localAuth.EntitlementToken);
            restRequest.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            restRequest.AddHeader("X-Riot-ClientVersion", _localAuth.version ?? "");
            IRestResponse restResponse = obj.Execute(restRequest);
            System.Diagnostics.Debug.WriteLine(restResponse.Content);
            MatchData matchData = JsonConvert.DeserializeObject<MatchData>(restResponse.Content) ?? new MatchData();
            matchData.StatusCode = (int)restResponse.StatusCode;
            return matchData;
        }

        return new MatchData();
    }

    public static String GetCoreMatchId()
    {
        return CoreGetPlayer.GetPlayer(_localAuth).MatchID;
    }

    public static String GetPreMatchId()
    {
        return PregameGetPlayer.GetPlayer(_localAuth).MatchID;
    }

    public static string GetCoreMatchMap(string matchId)
    {
        var coreMatchData = CoreGetMatch.GetMatch(_localAuth, matchId);
        Console.WriteLine(coreMatchData.ModeID);
        return coreMatchData.ModeID;
    }
}