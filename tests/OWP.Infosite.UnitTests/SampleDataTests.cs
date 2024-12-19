namespace OWP.Infosite.UnitTests;

using Models;

public static class SampleDataTests
{
    public static readonly List<CommonMatchModel> ListCommonMatches =
    [
        new()
        {
            ext_info = new SoccerEventSchedule
                { rd_checked = true, sport_id = 1, update_time = new DateTime(2000, 1, 1) },
            game_date = DateTime.Now,
            update_timestamp = new DateTime(2018, 1, 1, 8, 0, 0),
            league_name = "Test League",
            home_name = "Test Home",
            away_name = "Test Away",
            ibc_league_id = 1,
            ibc_home_id = null,
            ibc_away_id = null
        }
    ];

    public static readonly List<MatchModelQuery180Day> ListMatchModelQuery180Day =
    [
        new()
        {
            global_datetime = new DateTime(2018, 1, 1, 8, 0, 0),
            ibc_match_id = 1,
            info_match_id = 100,
            is_neutral = 1,
            team_away_name = "A",
            team_home_name = "H",
            league_name = "Test League"
        }
    ];

    public static readonly List<CommonMatchModel> EmptyListCommonMatches = [];

    public static readonly List<MatchModelQuery180Day> EmptyListMatchModelQuery180Day = [];

    public static readonly List<MatchRelation> ListMatchRelation =
    [
        new()
        {
            match_id = 100,
            parent_match_id = 222
        }
    ];
}