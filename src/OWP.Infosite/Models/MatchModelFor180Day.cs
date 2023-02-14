namespace OWP.Infosite.Models;

using Extensions;
using Olympus.Xml;

[Serializable]
public class MatchModelFor180Day
{
    public int MatchId { get; set; }
    
    [XmlName("KOTime")]
    public DateTime KoTime { get; set; }

    public string LeagueName { get; set; }

    public string HomeName { get; set; }

    public string AwayName { get; set; }

    public int AdminSiteId { get; set; }

    public bool IsNeutral { get; set; }

    public DateTime UpdatedTime { get; set; } = new(Year, 1, 1);

    private const int Year = 2000;

    public MatchModelFor180Day(MatchModelQuery180Day infositeMatch)
    {
        MatchId = infositeMatch.info_match_id;
        KoTime = infositeMatch.global_datetime.ToUtc8();
        LeagueName = infositeMatch.league_name;
        HomeName = infositeMatch.team_home_name;
        AwayName = infositeMatch.team_away_name;
        AdminSiteId = infositeMatch.ibc_match_id;
        IsNeutral = infositeMatch.is_neutral is 1;
    }
}