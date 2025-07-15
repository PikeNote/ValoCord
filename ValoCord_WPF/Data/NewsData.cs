namespace ValoCord_WPF.Data;

public class Action
{
    public Payload payload { get; set; }
}

public class Description
{
    public string type { get; set; }
    public string body { get; set; }
}

public class NewsMedia
{
    public string url { get; set; }
}

public class Payload
{
    public string url { get; set; }
    public bool? isExternal { get; set; }
}


public class NewsData
{
    public string title { get; set; }
    public string publishedAt { get; set; }
    public Action action { get; set; }
    public NewsMedia media { get; set; }
    public Description description { get; set; }
}