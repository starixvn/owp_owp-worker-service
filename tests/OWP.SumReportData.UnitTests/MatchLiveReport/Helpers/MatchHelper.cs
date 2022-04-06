using OWP.SumReportData.MatchLiveReport.Helpers;
using OWP.SumReportData.MatchLiveReport.Constants;
using Xunit;
using OWP.SumReportData.MatchLiveReport.Models;
using Xunit.Asserts.Compare;

namespace OWP.SumReportData.UnitTests.MatchLiveReport.Helpers;

public class MatchHelperUnitTests
{
    [Theory()]
    [InlineData("SABA ABC PES ", true)]
    [InlineData("SABA ABC PES - 5mins", true)]
    [InlineData("SABA ABC fifa - 5mins", false)]
    public void IsPes_WithPesLeague_ShouldReturnTrue(string owpLeagueName, bool result)
    {
        Assert.Equal(result, MatchHelper.IsPes(owpLeagueName));
    }

    [Theory()]
    [InlineData("SABA ABC FIFA ", true)]
    [InlineData("SABA ABC FIFA - 5mins", true)]
    [InlineData("SABA ABC PES - 5mins", false)]
    public void IsFifa_WithFifaLeague_ShouldReturnTrue(string owpLeagueName, bool result)
    {
        Assert.Equal(result, MatchHelper.IsFifa(owpLeagueName));
    }

    [Theory()]
    [InlineData("SABA ABC PES - 15 Mins ", BetTypeName.SabaClubFriendlyVirtualPes2115MinsPlay)]
    [InlineData("SABA ABC PES - 5 mins ", BetTypeName.SabaEliteFriendlyVirtualPes215MinsPlay)]
    [InlineData("SABA ABC PES - 20 mins ", BetTypeName.SabaInternationalFriendlyVirtualPes2120MinsPlay)]
    public void GetPesBetType_WithPesLeague_ShouldReturnCorrectBetTypeName(string owpLeagueName, BetTypeName result)
    {
        Assert.Equal(result, MatchHelper.GetPesBetType(owpLeagueName));
    }

    [Theory()]
    [InlineData("SABA ABC FIFA - 16 Mins ", BetTypeName.SabaClubFriendlyVirtualFifa2116MinsPlay)]
    [InlineData("SABA ABC FIFA - 6 mins ", BetTypeName.SabaClubFriendlyVirtualFifa20Futsal6MinsPlay)]
    [InlineData("M88 SABA ABC FIFA - 16 mins ", BetTypeName.M88SabaSpanishLeagueVirtualFifa21Season1016MinsPlay)]
    public void GetFifaBetType_WithFifaLeague_ShouldReturnCorrectBetTypeName(string owpLeagueName, BetTypeName result)
    {
        Assert.Equal(result, MatchHelper.GetFifaBetType(owpLeagueName));
    }

    [Fact]
    public void CategorizeSabaMatches_WithSabaLeague_ShouldReturnCategorizedMatches()
    {
        var matches = new List<OWPMatch>{
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC FIFA - 16 Mins "
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC FIFA - 6 mins "
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "M88 SABA ABC FIFA - 16 mins "
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC PES - 15 Mins "
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "ABC PES - 15 Mins "
            }
        };

        var expected = new List<OWPMatch>{
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC FIFA - 16 Mins ",
                BetTypeName = BetTypeName.SabaClubFriendlyVirtualFifa2116MinsPlay
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC FIFA - 6 mins ",
                BetTypeName = BetTypeName.SabaClubFriendlyVirtualFifa20Futsal6MinsPlay
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "M88 SABA ABC FIFA - 16 mins ",
                BetTypeName = BetTypeName.M88SabaSpanishLeagueVirtualFifa21Season1016MinsPlay
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC PES - 15 Mins ",
                BetTypeName = BetTypeName.SabaClubFriendlyVirtualPes2115MinsPlay
            }
        };

        DeepAssert.Equal(expected, MatchHelper.CategorizeSabaMatches(matches).ToList());
    }

    [Theory()]
    [InlineData("abc fantasy ", BetTypeName.Fantasy)]
    [InlineData("AUSTRIA REGIONAL LEAGUE MITTE", BetTypeName.Main)]
    [InlineData("AUSTRIA REGIONAL WHICH TEAM TO KICK OFF", BetTypeName.WhichTeamToKickOff)]
    public void GetMainCategory_WithMainLeague_ShouldReturnCorrectBetTypeName(string owpLeagueName, BetTypeName result)
    {
        Assert.Equal(result, MatchHelper.GetMainCategory(owpLeagueName));
    }

    [Fact]
    public void CategorizeMainMatches_WithMainLeague_ShouldReturnCategorizedMatches()
    {
        var matches = new List<OWPMatch>{
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "abc fantasy "
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL LEAGUE MITTE"
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL WHICH TEAM TO KICK OFF"
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC PES - 15 Mins "
            }
        };

        var expected = new List<OWPMatch>{
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "abc fantasy ",
                BetTypeName = BetTypeName.Fantasy
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL LEAGUE MITTE",
                BetTypeName = BetTypeName.Main
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL WHICH TEAM TO KICK OFF",
                BetTypeName = BetTypeName.WhichTeamToKickOff
            }
        };

        DeepAssert.Equal(expected, MatchHelper.CategorizeMainMatches(matches).ToList());
    }

    [Fact]
    public void CategorizeMinorMatches_WithMinorMatches_ShouldReturnCategorizedMatches()
    {
        var matches = new List<OWPMatch>{
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL LEAGUE MITTE"
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL WHICH TEAM TO KICK OFF"
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC PES - 15 Mins "
            }
        };

        var expected = new List<OWPMatch>{
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "abc fantasy ",
                BetTypeName = BetTypeName.Fantasy
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL LEAGUE MITTE",
                BetTypeName = BetTypeName.Main
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL WHICH TEAM TO KICK OFF",
                BetTypeName = BetTypeName.WhichTeamToKickOff
            }
        };

        DeepAssert.Equal(expected, MatchHelper.CategorizeMainMatches(matches).ToList());
    }
}