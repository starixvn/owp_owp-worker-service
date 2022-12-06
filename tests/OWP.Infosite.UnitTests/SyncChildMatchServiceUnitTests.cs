using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using NSubstitute;
using Olympus.InfoSiteClient.Models;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;
using OWP.Infosite.Services;
using OWP.Infosite.Settings;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.Infosite.UnitTests;

public class SyncChildMatchServiceUnitTests
{
    private readonly IDynamicRepository repository;
    private readonly SyncChildMatchServiceImp service;

    public SyncChildMatchServiceUnitTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        var infositeSettings = Options.Create(new InfoSiteSettings { InfositeAPI = "url" });
        service = new SyncChildMatchServiceImp(infositeSettings, repository);
    }

    [Fact]
    public void Sync()
    {
        var childMatches = MockChildMatch();
        var expected = MockExpected();

        var actual = service.MapChildMatches(childMatches);

        DeepAssert.Equal(expected, actual.ToList());
    }

    [Fact]
    public async Task SaveInfoSiteChildMatchCommand()
    {
        var matches = new List<MappedChildMatch>()
        {
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo Next Corners",
                AwayName = "Kyoto Sanga Next Corners",
                LeagueName = "JAPAN LEAGUE CUP - CORNERS",
                ParentInfoSiteMatchId = 1,
                EventDateTime = new DateTime(2022,04,15,9,0,0),
                UpdatedTime = new DateTime(2022,04,15,9,0,0),
                MinorBettypeId = -4,
                SubType = 2100,
            }
        };

        await service.SaveInfoSiteChildMatchCommand(matches);

        await repository.Received().ExecuteAsync(Arg.Is<PushInfoSiteChildMatchCommand>(x => x.ChildMatchesJson.Contains("JAPAN LEAGUE CUP - CORNERS")));
    }

    public IList<ChildMatch> MockChildMatch()
        => new List<ChildMatch>
        {
            new ChildMatch
            {
                home_name = "Hokkaido Consadole Sapporo Next Corners",
                away_name = "Kyoto Sanga Next Corners",
                league_name = "JAPAN LEAGUE CUP - CORNERS",
                match_id = 1,
                game_date = new DateTime(2022,04,15,1,0,0),
                sub_type = 2101,
                main_type = 1,
                stamp = new DateTime(2022,04,15,1,0,0),
            },
            new ChildMatch
            {
                home_name = "Hokkaido Consadole Sapporo Next booking",
                away_name = "Kyoto Sanga Next booking",
                league_name = "JAPAN LEAGUE CUP - BOOKING",
                match_id = 1,
                game_date = new DateTime(2022,04,15),
                sub_type = null,
                main_type = 1,
                stamp = new DateTime(2022,04,15),
            },
            new ChildMatch
            {
                home_name = "Hokkaido Consadole Sapporo Next booking",
                away_name = "Kyoto Sanga Next booking",
                league_name = "JAPAN LEAGUE CUP - BOOKING",
                match_id = 1,
                game_date = new DateTime(2022,04,15),
                sub_type = 0,
                stamp = new DateTime(2022,04,15),
                main_type = 1,
            },
            new ChildMatch
            {
                home_name = "Hokkaido Consadole Sapporo  00:00-15:00",
                away_name = "Kyoto Sanga  00:00-15:00",
                league_name = "JAPAN LEAGUE CUP - SPECIFIC 15 MINS",
                match_id = 2,
                game_date = new DateTime(2022,04,15),
                sub_type = 12345,
                main_type = null,
                stamp = new DateTime(2022,04,15)
            },
            new ChildMatch
            {
                home_name = "Home A",
                away_name = "Away A",
                league_name = "JAPAN LEAGUE CUP - SPECIFIC 15 MINS",
                match_id = 2,
                game_date = new DateTime(2022,04,15),
                stamp = new DateTime(2022,04,15),
                sub_type = 801,
                main_type = 1
            },
            new ChildMatch
            {
                home_name = "Home A",
                away_name = "Away A",
                league_name = "JAPAN LEAGUE CUP - SPECIFIC 15 MINS",
                match_id = 2,
                game_date = new DateTime(2022,04,15),
                stamp = new DateTime(2022,04,15),
                sub_type = 610,
                main_type = 1
            },
            new ChildMatch
            {
                home_name = "A",
                away_name = "B",
                league_name = "C",
                match_id = 2,
                game_date = new DateTime(2022,04,15),
                stamp = new DateTime(2022,04,15),
                sub_type = 12345,
                main_type = 111
            },
            new ChildMatch { match_id = 111, home_name = "League A (ET)" },
            new ChildMatch { match_id = 222, home_name = "League B (PEN)" },
        };

    public IList<MappedChildMatch> MockExpected()
        => new List<MappedChildMatch>
        {
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo Next Corners",
                AwayName = "Kyoto Sanga Next Corners",
                LeagueName = "JAPAN LEAGUE CUP - CORNERS",
                ParentInfoSiteMatchId = 1,
                EventDateTime = new DateTime(2022,04,15,9,0,0),
                UpdatedTime = new DateTime(2022,04,15,9,0,0),
                MinorBettypeId = -4,
                SubType = 2101,
                RepresentativeSubtype = 2100
            },
            new MappedChildMatch
            {
                HomeName = "Home A",
                AwayName = "Away A",
                LeagueName = "JAPAN LEAGUE CUP - SPECIFIC 15 MINS",
                ParentInfoSiteMatchId = 2,
                EventDateTime = new DateTime(2022,04,15,8,0,0),
                UpdatedTime = new DateTime(2022,04,15,8,0,0),
                MinorBettypeId= -15,
                SubType = 801,
                RepresentativeSubtype = 800
            },
            new MappedChildMatch
            {
                HomeName = "Home A",
                AwayName = "Away A",
                LeagueName = "JAPAN LEAGUE CUP - SPECIFIC 15 MINS",
                ParentInfoSiteMatchId = 2,
                EventDateTime = new DateTime(2022,04,15,8,0,0),
                UpdatedTime = new DateTime(2022,04,15,8,0,0),
                MinorBettypeId= -14,
                SubType = 610,
                RepresentativeSubtype = 600
            }
        };
}