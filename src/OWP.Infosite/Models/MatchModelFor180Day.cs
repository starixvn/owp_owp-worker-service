using OWP.Core.Attributes;
using OWP.Core.Extensions;

namespace OWP.Infosite.Models;

[Serializable]
[XmlName("Row")]
public class MatchModelFor180Day
{
    public int MatchId { get; set; }

    public DateTime KOTime { get; set; }

    public string LeagueName { get; set; }

    public string HomeName { get; set; }

    public string AwayName { get; set; }

    public int AdminSiteId { get; set; }

    public bool IsNeutral { get; set; }

    public DateTime UpdatedTime { get; set; } = new DateTime(2000, 1, 1);

    public MatchModelFor180Day(MatchModelQuery180Day infositeMatch)
    {
        MatchId = infositeMatch.info_match_id;
        KOTime = infositeMatch.global_datetime.ToUtc8();
        LeagueName = infositeMatch.league_name;
        HomeName = infositeMatch.team_home_name;
        AwayName = infositeMatch.team_away_name;
        AdminSiteId = infositeMatch.ibc_match_id;
        IsNeutral = infositeMatch.is_neutral.HasValue && infositeMatch.is_neutral.Value == 1;
    }
}