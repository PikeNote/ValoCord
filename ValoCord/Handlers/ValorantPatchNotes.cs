using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValoCord.Data;

namespace ValoCord.Handlers;

public static class ValorantPatchNotes
{
    public static List<NewsData> FetchLatestPatch()
    {
        List<Patches> patchList = new();
        var url = "https://playvalorant.com/en-us/news/";
        var web = new HtmlWeb();
        var doc = web.Load(url);
        
        List<NewsData> parsedNewsData = new();
        var newsJson =  doc.DocumentNode.SelectSingleNode("/html/body/script[1]").InnerText;
        var newsObjects = JObject.Parse(newsJson)["props"]["pageProps"]["page"]["blades"][2]["items"].Children().ToList();
            
        foreach (JToken result in newsObjects)
        {
            NewsData searchResult = result.ToObject<NewsData>();
            parsedNewsData.Add(searchResult);
        }

        return parsedNewsData.Slice(0,7);
    }
}

public class Patches
{
    public string PatchName { get; set; }
    public string Description { get; set; }
    public string Date { get; set; }
    public string ImageUrl { get; set; }
    public string RedirectURL { get; set; }
}