namespace OWP.Infosite.Models;

public class CommonSubMatch
{
    // id of match of the source provider
    public int match_id { get; set; }

    // id of source provider
    public int? website_id { get; set; }

    // is import match (other source should import as info site match then able to create admin site)
    public bool is_import { get; set; }

    // is virtual match (to distinguish soccerway or virtual match/other source)
    public bool is_virtual { get; set; }

    public int infosite_match_id { get; set; }

    // mbc match id if already create to admin site
    public int create_match_id { get; set; }

    public DateTime? game_date { get; set; }

    public string match_url { get; set; }

    // original match id before import (to look up the raw source match in imported match)
    public int ori_match_id { get; set; }

    // import match id after import (to look up the info site match in raw source match)
    public int imp_match_id { get; set; }

    public bool? is_postponed { get; set; }

    // Null: Normal, 0: Normal, 1: Postponed, 2: Cancelled, 3: No kick off time
    public int? match_status { get; set; }

    public int? coverage_id { get; set; }
}

public class CommonMatch : CommonSubMatch
{
    // testing match
    public bool is_test { get; set; }

    #region league team id/name info

    public int? ibc_league_id { get; set; }

    public int? league_id { get; set; }

    public string league_name { get; set; }

    public int? ibc_home_id { get; set; }

    public int? home_id { get; set; }

    public string home_name { get; set; }

    public int? ibc_away_id { get; set; }

    public int? away_id { get; set; }

    public string away_name { get; set; }

    #endregion league team id/name info

    #region league team attribute info

    public bool is_league_black { get; set; }
    public bool is_home_black { get; set; }
    public bool is_away_black { get; set; }
    public bool is_league_ignore { get; set; }
    public bool is_home_ignore { get; set; }
    public bool is_away_ignore { get; set; }
    public bool is_league_bad_ranking { get; set; }
    public bool is_home_bad_ranking { get; set; }
    public bool is_away_bad_ranking { get; set; }
    public bool is_home_downgrade { get; set; }
    public bool is_away_downgrade { get; set; }
    public bool is_home_new { get; set; }
    public bool is_away_new { get; set; }

    #endregion league team attribute info

    #region system statistics info

    // Round number or tag
    public string round { get; set; }

    // Round total number or tag
    public string round_total { get; set; }

    public string league_level3_name { get; set; }

    // the last update stamp in cache
    public DateTime? update_timestamp { get; set; }

    // merge matches: match_keys split with comma
    public string merge_match { get; set; }

    // Home round number.(InfoSite provide)
    public int home_game_no { get; set; }

    // Away round number.(InfoSite provide)
    public int away_game_no { get; set; }

    // Round total number.(InfoSite provide)
    public int team_matches { get; set; }

    // 0: cup, 1: regular, 2: final, 3: play off, 4: promotion relegation

    #endregion system statistics info

    #region other configs

    public int? league_type { get; set; }

    // refer to mbc league_id
    public int home_main_league_id { get; set; }

    // main league name
    public string home_main_league_name { get; set; }

    // refer to mbc league_id
    public int away_main_league_id { get; set; }

    // main league name
    public string away_main_league_name { get; set; }

    public bool localize_checked { get; set; }

    #endregion other configs
}

public class CommonMatchModel : CommonMatch
{
    // user config
    public SoccerEventSchedule ext_info { get; set; }

    // list of merged match
    public List<CommonSubMatch> ls_merge_match { get; set; }
}