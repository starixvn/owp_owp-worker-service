using OWP.Infosite.Models;

namespace OWP.Infosite.UnitTests;

public static class SampleDataTests
{
    public static readonly List<CommonMatchModel> ListCommonMatches = new List<CommonMatchModel>
    {
        new CommonMatchModel
        {
            ext_info = new SoccerEventSchedule
                { rd_checked = true, sport_id = 1, update_time = new DateTime(2000, 1, 1) },
            game_date = DateTime.Now,
            update_timestamp = new DateTime(2018, 1, 1, 8, 0, 0),
            league_name = "Test League",
            home_name = "Test Home",
            away_name = "Test Away",
            ibc_league_id = 1,
            ibc_home_id = 1,
            ibc_away_id = 1
        }
    };

    public static readonly List<MatchModelQuery180Day> ListMatchModelQuery180Day = new List<MatchModelQuery180Day>
    {
        new MatchModelQuery180Day
        {
            global_datetime = new DateTime(2018, 1, 1, 8, 0, 0),
            ibc_match_id = 1,
            info_match_id = 100,
            is_neutral = 1,
            team_away_name = "A",
            team_home_name = "H",
            league_name = "Test League"
        }
    };

    public static readonly List<CommonMatchModel> EmptyListCommonMatches = new List<CommonMatchModel>();

    public static readonly List<MatchModelQuery180Day> EmptyListMatchModelQuery180Day = new List<MatchModelQuery180Day>();

    public static readonly List<MatchRelation> ListMatchRelation = new List<MatchRelation>
    {
        new MatchRelation
        {
            match_id = 100,
            parent_match_id = 222
        }
    };
}