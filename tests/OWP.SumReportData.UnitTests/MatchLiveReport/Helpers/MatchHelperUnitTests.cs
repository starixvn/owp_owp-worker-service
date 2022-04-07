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
            ,
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "ABC PES - 15 Mins ",
                MinorBettypeId = -2
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
            ,
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "ABC PES - 15 Mins ",
                MinorBettypeId = -3
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
                MinorBettypeId = 1000
            },
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15Mins
            },
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific10Mins
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Corners
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextCorners
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15MinsNumberOfCorners
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Bookings
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextBookings
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15MinsTotalBookings
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.WhichTeamToTakeTheFirstPenalty
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Winner
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastCorners
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastBookings
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOU
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOU
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOU
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOU
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FirstPenaltyToScore
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FirstPenaltyAwarded
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextGoalKick
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastGoalKick
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.GoalKick
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Offside
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastThrowIn
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.ThrowIn
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastFreeKick
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FreeKick
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.SpecialGoal
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut
            }
            ,new OWPMatch{
                MinorBettypeId = null
            }
        };

        var expected = new List<OWPMatch>{
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15Mins,
                BetTypeName = BetTypeName.Specific15Mins
            },
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific10Mins,
                BetTypeName = BetTypeName.Specific10Mins
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Corners,
                BetTypeName = BetTypeName.Corners
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextCorners,
                BetTypeName = BetTypeName.NextCorners
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15MinsNumberOfCorners,
                BetTypeName = BetTypeName.Specific15MinsNumberOfCorners
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Bookings,
                BetTypeName = BetTypeName.Bookings
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextBookings,
                BetTypeName = BetTypeName.NextBookings
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15MinsTotalBookings,
                BetTypeName = BetTypeName.Specific15MinsTotalBookings
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.WhichTeamToTakeTheFirstPenalty,
                BetTypeName = BetTypeName.WhichTeamToTakeTheFirstPenalty
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Winner,
                BetTypeName = BetTypeName.Winner
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastCorners,
                BetTypeName = BetTypeName.LastCorners
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastBookings,
                BetTypeName = BetTypeName.LastBookings
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOU,
                BetTypeName = BetTypeName.SingleTeamOU
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOU,
                BetTypeName = BetTypeName.SingleTeamOU
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOU,
                BetTypeName = BetTypeName.SingleTeamOU
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOU,
                BetTypeName = BetTypeName.SingleTeamOU
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FirstPenaltyToScore,
                BetTypeName = BetTypeName.FirstPenaltyToScore
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FirstPenaltyAwarded,
                BetTypeName = BetTypeName.FirstPenaltyAwarded
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextGoalKick,
                BetTypeName = BetTypeName.NextGoalKick
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastGoalKick,
                BetTypeName = BetTypeName.LastGoalKick
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.GoalKick,
                BetTypeName = BetTypeName.GoalKick
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Offside,
                BetTypeName = BetTypeName.Offside
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastThrowIn,
                BetTypeName = BetTypeName.LastThrowIn
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.ThrowIn,
                BetTypeName = BetTypeName.ThrowIn
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastFreeKick,
                BetTypeName = BetTypeName.LastFreeKick
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FreeKick,
                BetTypeName = BetTypeName.FreeKick
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.SpecialGoal,
                BetTypeName = BetTypeName.SpecialGoal
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut
            }
        };

        DeepAssert.Equal(expected, MatchHelper.CategorizeMinorMatches(matches).ToList());
    }

    [Fact]
    public void SumMatches_WithMatches_ShouldReturnReportModel()
    {
        var eventDate = DateTime.Today;

        var matches = new List<OWPMatch>
        {
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15Mins,
                BetTypeName = BetTypeName.Specific15Mins,
                LeagueCategory = "MJ"
            },
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific10Mins,
                BetTypeName = BetTypeName.Specific10Mins,
                LeagueCategory = "MJ"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Corners,
                BetTypeName = BetTypeName.Corners,
                LeagueCategory = "MJ"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextCorners,
                BetTypeName = BetTypeName.NextCorners,
                LeagueCategory = "MJ"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15MinsNumberOfCorners,
                BetTypeName = BetTypeName.Specific15MinsNumberOfCorners,
                LeagueCategory = "M1"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Bookings,
                BetTypeName = BetTypeName.Bookings,
                LeagueCategory = "M1"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextBookings,
                BetTypeName = BetTypeName.NextBookings,
                LeagueCategory = "M1"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15MinsTotalBookings,
                BetTypeName = BetTypeName.Specific15MinsTotalBookings,
                LeagueCategory = "M1"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.WhichTeamToTakeTheFirstPenalty,
                BetTypeName = BetTypeName.WhichTeamToTakeTheFirstPenalty,
                LeagueCategory = "Others 1"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Winner,
                BetTypeName = BetTypeName.Winner,
                LeagueCategory = "Others 2"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastCorners,
                BetTypeName = BetTypeName.LastCorners,
                LeagueCategory = "Others 2"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastBookings,
                BetTypeName = BetTypeName.LastBookings,
                LeagueCategory = "Others 3"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOU,
                BetTypeName = BetTypeName.SingleTeamOU,
                LeagueCategory = "Others 4"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOU,
                BetTypeName = BetTypeName.SingleTeamOU,
                LeagueCategory = "Others 4"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOU,
                BetTypeName = BetTypeName.SingleTeamOU,
                LeagueCategory = "Others 4"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOU,
                BetTypeName = BetTypeName.SingleTeamOU,
                LeagueCategory = "Others 4"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FirstPenaltyToScore,
                BetTypeName = BetTypeName.FirstPenaltyToScore,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FirstPenaltyAwarded,
                BetTypeName = BetTypeName.FirstPenaltyAwarded,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextGoalKick,
                BetTypeName = BetTypeName.NextGoalKick,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastGoalKick,
                BetTypeName = BetTypeName.LastGoalKick,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.GoalKick,
                BetTypeName = BetTypeName.GoalKick,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Offside,
                BetTypeName = BetTypeName.Offside,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastThrowIn,
                BetTypeName = BetTypeName.LastThrowIn,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.ThrowIn,
                BetTypeName = BetTypeName.ThrowIn,
                LeagueCategory = "Others 6"
            }
            ,new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastFreeKick,
                BetTypeName = BetTypeName.LastFreeKick,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FreeKick,
                BetTypeName = BetTypeName.FreeKick,
                LeagueCategory = "Unknown"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.SpecialGoal,
                BetTypeName = BetTypeName.SpecialGoal,
                LeagueCategory = "Unknown"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "Others 6"
            }
        };

        var expected = new List<ReportModel> {
            new ReportModel {
                EventDate = eventDate,
                BetType = "15 mins (4 quarters)",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
            new ReportModel {
                EventDate = eventDate,
                BetType = "10 mins (6 quarters)",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
            new ReportModel {
                EventDate = eventDate,
                BetType = "Corner",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
            new ReportModel {
                EventDate = eventDate,
                BetType = "Next corners",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "15 mins corners",
                IsSaba = false,
                LeagueCategory = "M1",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Bookings",
                IsSaba = false,
                LeagueCategory = "M1",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Next bookings",
                IsSaba = false,
                LeagueCategory = "M1",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "15 mins bookings",
                IsSaba = false,
                LeagueCategory = "M1",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Which team to take the first penalty",
                IsSaba = false,
                LeagueCategory = "Others 1",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Winner",
                IsSaba = false,
                LeagueCategory = "Others 2",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Last corners",
                IsSaba = false,
                LeagueCategory = "Others 2",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Last bookings",
                IsSaba = false,
                LeagueCategory = "Others 3",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Single Team Home/Away (2 events)",
                IsSaba = false,
                LeagueCategory = "Others 4",
                MatchCount = 4
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "1st Penalty To Score",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "1st Penalty Awarded",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Next Goal kick",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Last Goal kick",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Goal kick (2H 1st xxx)",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Offside (2H 1st xxx)",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Last Throw In",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Throw In (2H 1st xxx)",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Last Free kick",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Free kick (2H 1st xxx)",
                IsSaba = false,
                LeagueCategory = "Unknown",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Special goal",
                IsSaba = false,
                LeagueCategory = "Unknown",
                MatchCount = 1
            }
            ,
            new ReportModel {
                EventDate = eventDate,
                BetType = "Penalty shoot out Home/Away (2 events)",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 4
            }
        };

        DeepAssert.Equal(expected, MatchHelper.SumMatches(eventDate, matches).ToList());
    }
}