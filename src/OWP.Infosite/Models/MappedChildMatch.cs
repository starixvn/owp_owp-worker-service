namespace OWP.Infosite.Models;

public class MappedChildMatch
{
    public int ParentInfoSiteMatchId { get; set; }

    public int? AdminSiteMatchId { get; set; }

    public int? MinorBettypeId { get; set; }

    public int? SubType { get; set; }

    public string LeagueName { get; set; }

    public string HomeName { get; set; }

    public string AwayName { get; set; }

    public DateTime EventDateTime { get; set; }

    public DateTime UpdatedTime { get; set; }
}