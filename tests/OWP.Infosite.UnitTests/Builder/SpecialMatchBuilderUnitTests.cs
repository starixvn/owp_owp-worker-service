namespace OWP.Infosite.UnitTests.Builder;

using Infosite.Builder;
using Models;
using Xunit;
using Xunit.Asserts.Compare;

public class SpecialMatchBuilderUnitTests
{
    [Fact]
    public void ReduceSpecialMatch_Always_ReturnMappedChildMatch()
    {
        var expected = MockExpected();

        var actual = new SpecialMatchBuilder().ReduceSpecialMatch(MockMappedChildMatch());

        DeepAssert.Equal(expected, actual);
    }

    public IEnumerable<MappedChildMatch> MockMappedChildMatch()
        => new List<MappedChildMatch>
        {
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo 3rd Corner",
                AwayName = "Kyoto Sanga 3rd Corner",
                LeagueName = "JAPAN LEAGUE CUP - CORNERS",
                MinorBettypeId = -4,
                ParentInfoSiteMatchId = 1,
                EventDateTime = new DateTime(2022,04,15),
            },
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo 3rd Corner",
                AwayName = "Kyoto Sanga 3rd Corner",
                LeagueName = "JAPAN LEAGUE CUP - CORNERS",
                MinorBettypeId = -4,
                ParentInfoSiteMatchId = 1,
                EventDateTime = new DateTime(2022,04,15),
            },
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo 3rd booking",
                AwayName = "Kyoto Sanga 3rd booking",
                LeagueName = "JAPAN LEAGUE CUP - BOOKING",
                MinorBettypeId = -7,
                ParentInfoSiteMatchId = 1,
                EventDateTime = new DateTime(2022,04,15),
            },
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo 4rd booking",
                AwayName = "Kyoto Sanga 4rd booking",
                LeagueName = "JAPAN LEAGUE CUP - BOOKING",
                MinorBettypeId = -7,
                ParentInfoSiteMatchId = 1,
                EventDateTime = new DateTime(2022,04,15),
            },
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo",
                AwayName = "Kyoto Sanga ",
                LeagueName = "JAPAN LEAGUE CUP",
                MinorBettypeId = null,
                ParentInfoSiteMatchId = 1,
                EventDateTime = new DateTime(2022,04,15),
            },
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo  00:00-15:00",
                AwayName = "Kyoto Sanga  00:00-15:00",
                LeagueName = "JAPAN LEAGUE CUP - SPECIFIC 15 MINS",
                MinorBettypeId = -1,
                ParentInfoSiteMatchId = 2,
                EventDateTime = new DateTime(2022,04,15),
            }
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
                EventDateTime = new DateTime(2022,04,15),
                MinorBettypeId = -4,
            },
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo Next Booking",
                AwayName = "Kyoto Sanga Next Booking",
                LeagueName = "JAPAN LEAGUE CUP - BOOKING",
                ParentInfoSiteMatchId = 1,
                EventDateTime = new DateTime(2022,04,15),
                MinorBettypeId = -7,
            },
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo",
                AwayName = "Kyoto Sanga ",
                LeagueName = "JAPAN LEAGUE CUP",
                ParentInfoSiteMatchId = 1,
                EventDateTime = new DateTime(2022,04,15)
            },
            new MappedChildMatch
            {
                HomeName = "Hokkaido Consadole Sapporo  (SPECIFIC 15 MINS)",
                AwayName = "Kyoto Sanga  (SPECIFIC 15 MINS)",
                LeagueName = "JAPAN LEAGUE CUP - SPECIFIC 15 MINS",
                ParentInfoSiteMatchId = 2,
                EventDateTime = new DateTime(2022,04,15),
                MinorBettypeId = -1,
            }
        };
}
