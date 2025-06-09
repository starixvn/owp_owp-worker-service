namespace OWP.Provider.Share.AgentConfirm.Model;

public class AgentMatchInfo
{
    public int OWPMatchId { get; set; }

    public int MatchId { get; set; }

    public string MatchStatus { get; set; }

    public string Provider { get; set; }

    public bool IsSaba { get; set; }

    public string LstBookProvider { get; set; }
}
