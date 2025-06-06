using System.Threading.Tasks;
using NLog;

namespace ValoCord.Handlers;
using ValAPINet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public class ValorantAPI
{
    private static Auth localAuth;
    static Logger logger = LogManager.GetLogger("Valorant API");

    public static async Task<Auth> reAuthAttempt()
    {
        localAuth = Websocket.GetAuthLocal();
        logger.Info("User Authenticated");
        return localAuth;
    }
    
    public static void ResetAuth()
    {
        localAuth = null;
    }

    public static String getCurrentUser()
    {
        return localAuth.subject;
    }

    public static Boolean CheckAuth()
    {
        return localAuth == null;
    }

    public static MatchData? GetMatchData(String matchID)
    {
        //return MatchData.GetMatchData(localAuth, matchID);
        new MatchData();
        RestClient obj = new RestClient("https://pd." + localAuth.region.ToString() + ".a.pvp.net/match-details/v1/matches/" + matchID) {
            CookieContainer = localAuth.cookies
        };
        RestRequest restRequest = new RestRequest(Method.GET);
        restRequest.AddHeader("Authorization", "Bearer " + localAuth.AccessToken);
        restRequest.AddHeader("X-Riot-Entitlements-JWT", localAuth.EntitlementToken);
        restRequest.AddHeader("X-Riot-ClientPlatform", "ewU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
        restRequest.AddHeader("X-Riot-ClientVersion", localAuth.version ?? "");
        IRestResponse restResponse = obj.Execute(restRequest);
        System.Diagnostics.Debug.WriteLine(restResponse.Content);
        MatchData matchData = JsonConvert.DeserializeObject<MatchData>(restResponse.Content);
        matchData.StatusCode = (int)restResponse.StatusCode;
        return matchData;
    }

    public static String GetCoreMatchID()
    {
        return CoreGetPlayer.GetPlayer(localAuth).MatchID;
    }

    public static String GetPreMatchID()
    {
        return PregameGetPlayer.GetPlayer(localAuth).MatchID;
    }
}