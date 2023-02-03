using System.Threading.Tasks;
using NLog;

namespace ValCord.Handlers;
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
        // Copied from Valorant C# API (Added felxbility of adjusting the code)
        // Start attempt of verification
        string text = "";
        while (text == "") {
            try {
                using (FileStream stream2 = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Riot Games\\Riot Client\\Config\\lockfile", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader streamReader2 = new StreamReader(stream2, Encoding.Default))
                    text = streamReader2.ReadToEnd();
            } catch (Exception) {
            }
        }
        
        RestClient restClient = new RestClient("https://valorant-api.com/v1/version");
        RestRequest request = new RestRequest(Method.GET);
        JToken jToken = JObject.FromObject(JObject.FromObject(JsonConvert.DeserializeObject(restClient.Execute(request).Content))[(object)"data"]);
        string version = jToken["branch"].Value<string>() + "-shipping-" + jToken["buildVersion"].Value<string>() + "-" + jToken["version"].Value<string>().Substring(jToken["version"].Value<string>().Length - 6);
        string[] array = text.Split(":", StringSplitOptions.None);
        RestClient client = new RestClient(new Uri("https://127.0.0.1:" + array[2] + "/entitlements/v1/token")) {
            RemoteCertificateValidationCallback = (RemoteCertificateValidationCallback)((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true)
        };
        RestRequest restRequest = new RestRequest(Method.GET);
        restRequest.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("riot:" + array[3])));
        restRequest.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
        restRequest.AddHeader("X-Riot-ClientVersion", "release-02.06-shipping-14-540727");
        IRestResponse restResponse = client.Get(restRequest);
        JObject jObject = new JObject();
        if (!restResponse.IsSuccessful)
            return null;
        jObject = JObject.Parse(restResponse.Content);
        Auth auth = new Auth();
        auth.AccessToken = (string)jObject["accessToken"];
        auth.EntitlementToken = (string)jObject["token"];
        auth.subject = (string)jObject["subject"];
        auth.version = version;
        auth.cookies = new CookieContainer();
        RestClient client2 = new RestClient(new Uri("https://127.0.0.1:" + array[2] + "/player-affinity/product/v1/token")) {
            RemoteCertificateValidationCallback = (RemoteCertificateValidationCallback)((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true)
        };
        IRestRequest restRequest2 = new RestRequest(Method.POST);
        restRequest2.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("riot:" + array[3])));
        var value = new {
            product = "valorant"
        };
        restRequest2.AddJsonBody(JsonConvert.SerializeObject(value));
        string a = "";
        while(a=="")
        {
            String jsContent = client2.Post(restRequest2).Content;
            
            if (jsContent != "")
            {
                a= (string)JObject.Parse(jsContent)["affinities"]["live"];
            }
            await Task.Delay(1500);
        }

        if (a == "NA")
            auth.region = Region.NA;
        else if (a == "AP") {
            auth.region = Region.AP;
        } else if (a == "EU") {
            auth.region = Region.EU;
        } else if (a == "KO") {
            auth.region = Region.KO;
        }

        localAuth = auth;
        logger.Info("User Authenticated");
        return auth;
    }
    
    public static void ResetAuth()
    {
        localAuth = null;
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
        restRequest.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
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