namespace OWP.Infosite.Models;

public class RoundTwInfo
{
    public bool finish{get;set;}
    public bool status{get;set;}
    public string message{get;set;}
    public ContentTw[] content{get;set;}
}

public class ContentTw{
    public int match_id{get;set;}
    public string round_home_away_total{get;set;}
}