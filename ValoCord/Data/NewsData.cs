namespace ValoCord.Data;

public class Action
{
    public Payload Payload { get; set; } = new ();
}

public class Description
{
    public string Type { get; set; } = "";
    public string Body { get; set; } = "";
}

public class NewsMedia
{
    public string Url { get; set; } = "";
}

public class Payload
{
    public string url { get; set; } = "";
    public bool? IsExternal { get; set; } = false;
}


public class NewsData
{
    public string Title { get; set; } = "";
    public string PublishedAt { get; set; } = "";
    public Action Action { get; set; } = new();
    public NewsMedia Media { get; set; } = new();
    public Description Description { get; set; } = new();
}