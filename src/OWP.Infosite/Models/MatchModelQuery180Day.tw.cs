namespace OWP.Infosite.Models;

public class MatchModelQuery180Day
{
    public int info_match_id { get; set; }

    public DateTime global_datetime { get; set; }

    public string league_name { get; set; }

    public string team_home_name { get; set; }

    public string team_away_name { get; set; }

    public int ibc_match_id { get; set; }

    public int? is_neutral { get; set; }

    public bool is_team_reverse { get; set; }
}