namespace OWP.SumReport.UnitTests.MatchLiveReport.Helpers;

using SumReport.MatchLiveReport.Constants;
using SumReport.MatchLiveReport.Helpers;
using SumReport.MatchLiveReport.Models;
using Xunit;
using Xunit.Asserts.Compare;

public class MatchHelperUnitTests
{
    [Theory]
    [InlineData("SABA ABC PES ", true)]
    [InlineData("SABA ABC PES - 5mins", true)]
    [InlineData("SABA ABC fifa - 5mins", false)]
    public void IsPes_WithPesLeague_ShouldReturnTrue(string owpLeagueName, bool result)
    {
        Assert.Equal(result, MatchHelper.IsPes(owpLeagueName));
    }

    [Theory]
    [InlineData("SABA ABC FIFA ", true)]
    [InlineData("SABA ABC FIFA - 5mins", true)]
    [InlineData("SABA ABC PES - 5mins", false)]
    public void IsFifa_WithFifaLeague_ShouldReturnTrue(string owpLeagueName, bool result)
    {
        Assert.Equal(result, MatchHelper.IsFifa(owpLeagueName));
    }

    [Theory]
    [InlineData("SABA ABC PES - 15 Mins ", BetTypeName.SabaClubFriendlyVirtualPes2115MinsPlay)]
    [InlineData("SABA ABC PES - 5 mins ", BetTypeName.SabaEliteFriendlyVirtualPes215MinsPlay)]
    [InlineData("SABA ABC PES - 20 mins ", BetTypeName.SabaInternationalFriendlyVirtualPes2120MinsPlay)]
    public void GetPesBetType_WithPesLeague_ShouldReturnCorrectBetTypeName(string owpLeagueName, BetTypeName result)
    {
        Assert.Equal(result, MatchHelper.GetPesBetType(owpLeagueName));
    }

    [Theory]
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
        var matches = new List<OwpMatch>{
            new()
            {
                IsSaba = true,
                OwpLeagueName = "SABA ABC FIFA - 16 Mins "
            },
            new()
            {
                IsSaba = true,
                OwpLeagueName = "SABA ABC FIFA - 6 mins "
            },
            new()
            {
                IsSaba = true,
                OwpLeagueName = "M88 SABA ABC FIFA - 16 mins "
            },
            new()
            {
                IsSaba = true,
                OwpLeagueName = "SABA ABC PES - 15 Mins "
            },
            new()
            {
                IsSaba = false,
                OwpLeagueName = "ABC PES - 15 Mins "
            }
            ,
            new()
            {
                IsSaba = false,
                OwpLeagueName = "ABC PES - 15 Mins ",
                MinorBettypeId = -2
            }
        };

        var expected = new List<OwpMatch>{
            new()
            {
                IsSaba = true,
                OwpLeagueName = "SABA ABC FIFA - 16 Mins ",
                BetTypeName = BetTypeName.SabaClubFriendlyVirtualFifa2116MinsPlay
            },
            new()
            {
                IsSaba = true,
                OwpLeagueName = "SABA ABC FIFA - 6 mins ",
                BetTypeName = BetTypeName.SabaClubFriendlyVirtualFifa20Futsal6MinsPlay
            },
            new()
            {
                IsSaba = true,
                OwpLeagueName = "M88 SABA ABC FIFA - 16 mins ",
                BetTypeName = BetTypeName.M88SabaSpanishLeagueVirtualFifa21Season1016MinsPlay
            },
            new()
            {
                IsSaba = true,
                OwpLeagueName = "SABA ABC PES - 15 Mins ",
                BetTypeName = BetTypeName.SabaClubFriendlyVirtualPes2115MinsPlay
            }
        };

        DeepAssert.Equal(expected, MatchHelper.CategorizeSabaMatches(matches).ToList());
    }

    [Theory]
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
        var matches = new List<OwpMatch>{
            new()
            {
                IsSaba = false,
                OwpLeagueName = "abc fantasy "
            },
            new()
            {
                IsSaba = false,
                OwpLeagueName = "AUSTRIA REGIONAL LEAGUE MITTE"
            },
            new()
            {
                IsSaba = false,
                OwpLeagueName = "AUSTRIA REGIONAL WHICH TEAM TO KICK OFF"
            },
            new()
            {
                IsSaba = true,
                OwpLeagueName = "SABA ABC PES - 15 Mins "
            }
            ,
            new()
            {
                IsSaba = false,
                OwpLeagueName = "ABC PES - 15 Mins ",
                MinorBettypeId = -3
            }
        };

        var expected = new List<OwpMatch>{
            new()
            {
                IsSaba = false,
                OwpLeagueName = "abc fantasy ",
                BetTypeName = BetTypeName.Fantasy
            },
            new()
            {
                IsSaba = false,
                OwpLeagueName = "AUSTRIA REGIONAL LEAGUE MITTE",
                BetTypeName = BetTypeName.Main
            },
            new()
            {
                IsSaba = false,
                OwpLeagueName = "AUSTRIA REGIONAL WHICH TEAM TO KICK OFF",
                BetTypeName = BetTypeName.WhichTeamToKickOff
            }
        };

        DeepAssert.Equal(expected, MatchHelper.CategorizeMainMatches(matches).ToList());
    }

    [Fact]
    public void CategorizeMinorMatches_WithMinorMatches_ShouldReturnCategorizedMatches()
    {
        var mainMatches = new List<OwpMatch>
        {
            new()
            {   
                OwpMatchId = 1,
                LeagueCategory = "MJ"
            },
            new()
            {
                OwpMatchId = 2,
                LeagueCategory = "M1"
            },
            new()
            {
                OwpMatchId = 3,
                LeagueCategory = "M4"
            }
        };

        var matches = new List<OwpMatch>{
            new()
            {
                MinorBettypeId = 1000,
                ParentMatchId = 1
            },
            new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15Mins,
                ParentMatchId = 1
            },
            new()
            {
                MinorBettypeId = (int)MinorBetType.Specific10Mins,
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Corners,
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextCorners,
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15MinsNumberOfCorners,
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Bookings,
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextBookings,
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15MinsTotalBookings,
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.WhichTeamToTakeTheFirstPenalty,
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Winner,
                ParentMatchId = 2
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastCorners,
                ParentMatchId = 2
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastBookings,
                ParentMatchId = 2
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOu,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOu,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOu,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOu,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FirstPenaltyToScore,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FirstPenaltyAwarded,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextGoalKick,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastGoalKick,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.GoalKick,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Offside,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastThrowIn,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.ThrowIn,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastFreeKick,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FreeKick,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.SpecialGoal,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                ParentMatchId = 3
            }
            ,new()
            {
                MinorBettypeId = null
            }
        };

        var expected = new List<OwpMatch>{
            new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15Mins,
                BetTypeName = BetTypeName.Specific15Mins,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            },
            new()
            {
                MinorBettypeId = (int)MinorBetType.Specific10Mins,
                BetTypeName = BetTypeName.Specific10Mins,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Corners,
                BetTypeName = BetTypeName.Corners,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextCorners,
                BetTypeName = BetTypeName.NextCorners,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15MinsNumberOfCorners,
                BetTypeName = BetTypeName.Specific15MinsNumberOfCorners,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Bookings,
                BetTypeName = BetTypeName.Bookings,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextBookings,
                BetTypeName = BetTypeName.NextBookings,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15MinsTotalBookings,
                BetTypeName = BetTypeName.Specific15MinsTotalBookings,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.WhichTeamToTakeTheFirstPenalty,
                BetTypeName = BetTypeName.WhichTeamToTakeTheFirstPenalty,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Winner,
                BetTypeName = BetTypeName.Winner,
                LeagueCategory = "M1",
                ParentMatchId = 2
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastCorners,
                BetTypeName = BetTypeName.LastCorners,
                LeagueCategory = "M1",
                ParentMatchId = 2
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastBookings,
                BetTypeName = BetTypeName.LastBookings,
                LeagueCategory = "M1",
                ParentMatchId = 2
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOu,
                BetTypeName = BetTypeName.SingleTeamOu,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOu,
                BetTypeName = BetTypeName.SingleTeamOu,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOu,
                BetTypeName = BetTypeName.SingleTeamOu,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOu,
                BetTypeName = BetTypeName.SingleTeamOu,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FirstPenaltyToScore,
                BetTypeName = BetTypeName.FirstPenaltyToScore,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FirstPenaltyAwarded,
                BetTypeName = BetTypeName.FirstPenaltyAwarded,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextGoalKick,
                BetTypeName = BetTypeName.NextGoalKick,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastGoalKick,
                BetTypeName = BetTypeName.LastGoalKick,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.GoalKick,
                BetTypeName = BetTypeName.GoalKick,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Offside,
                BetTypeName = BetTypeName.Offside,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastThrowIn,
                BetTypeName = BetTypeName.LastThrowIn,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.ThrowIn,
                BetTypeName = BetTypeName.ThrowIn,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastFreeKick,
                BetTypeName = BetTypeName.LastFreeKick,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FreeKick,
                BetTypeName = BetTypeName.FreeKick,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.SpecialGoal,
                BetTypeName = BetTypeName.SpecialGoal,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "M4",
                ParentMatchId = 3
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "M4",
                ParentMatchId = 3
            }
        };

        DeepAssert.Equal(expected, MatchHelper.CategorizeMinorMatches(matches, mainMatches).ToList());
    }

    [Fact]
    public void SumMatches_WithMatches_ShouldReturnReportModel()
    {
        var eventDate = DateTime.Today;

        var matches = new List<OwpMatch>
        {
            new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15Mins,
                BetTypeName = BetTypeName.Specific15Mins,
                LeagueCategory = "MJ"
            },
            new()
            {
                MinorBettypeId = (int)MinorBetType.Specific10Mins,
                BetTypeName = BetTypeName.Specific10Mins,
                LeagueCategory = "MJ"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Corners,
                BetTypeName = BetTypeName.Corners,
                LeagueCategory = "MJ"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextCorners,
                BetTypeName = BetTypeName.NextCorners,
                LeagueCategory = "MJ"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15MinsNumberOfCorners,
                BetTypeName = BetTypeName.Specific15MinsNumberOfCorners,
                LeagueCategory = "M1"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Bookings,
                BetTypeName = BetTypeName.Bookings,
                LeagueCategory = "M1"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextBookings,
                BetTypeName = BetTypeName.NextBookings,
                LeagueCategory = "M1"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15MinsTotalBookings,
                BetTypeName = BetTypeName.Specific15MinsTotalBookings,
                LeagueCategory = "M1"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.WhichTeamToTakeTheFirstPenalty,
                BetTypeName = BetTypeName.WhichTeamToTakeTheFirstPenalty,
                LeagueCategory = "Others 1"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Winner,
                BetTypeName = BetTypeName.Winner,
                LeagueCategory = "Others 2"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastCorners,
                BetTypeName = BetTypeName.LastCorners,
                LeagueCategory = "Others 2"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastBookings,
                BetTypeName = BetTypeName.LastBookings,
                LeagueCategory = "Others 3"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOu,
                BetTypeName = BetTypeName.SingleTeamOu,
                LeagueCategory = "Others 4"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOu,
                BetTypeName = BetTypeName.SingleTeamOu,
                LeagueCategory = "Others 4"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOu,
                BetTypeName = BetTypeName.SingleTeamOu,
                LeagueCategory = "Others 4"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOu,
                BetTypeName = BetTypeName.SingleTeamOu,
                LeagueCategory = "Others 4"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FirstPenaltyToScore,
                BetTypeName = BetTypeName.FirstPenaltyToScore,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FirstPenaltyAwarded,
                BetTypeName = BetTypeName.FirstPenaltyAwarded,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextGoalKick,
                BetTypeName = BetTypeName.NextGoalKick,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastGoalKick,
                BetTypeName = BetTypeName.LastGoalKick,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.GoalKick,
                BetTypeName = BetTypeName.GoalKick,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Offside,
                BetTypeName = BetTypeName.Offside,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastThrowIn,
                BetTypeName = BetTypeName.LastThrowIn,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.ThrowIn,
                BetTypeName = BetTypeName.ThrowIn,
                LeagueCategory = "Others 6"
            }
            ,new()
            {
                MinorBettypeId = (int)MinorBetType.LastFreeKick,
                BetTypeName = BetTypeName.LastFreeKick,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FreeKick,
                BetTypeName = BetTypeName.FreeKick,
                LeagueCategory = "Unknown"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.SpecialGoal,
                BetTypeName = BetTypeName.SpecialGoal,
                LeagueCategory = "Unknown"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                BetTypeName = BetTypeName.PenaltyShootOut,
                LeagueCategory = "Others 6"
            }
        };

        var expected = new List<ReportModel> {
            new()
            {
                EventDate = eventDate,
                BetType = "15 mins (4 quarters)",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
            new()
            {
                EventDate = eventDate,
                BetType = "10 mins (6 quarters)",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
            new()
            {
                EventDate = eventDate,
                BetType = "Corners",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
            new()
            {
                EventDate = eventDate,
                BetType = "Next corners",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "15 mins corners",
                IsSaba = false,
                LeagueCategory = "M1",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Bookings",
                IsSaba = false,
                LeagueCategory = "M1",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Next bookings",
                IsSaba = false,
                LeagueCategory = "M1",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "15 mins bookings",
                IsSaba = false,
                LeagueCategory = "M1",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Which team to take the first penalty",
                IsSaba = false,
                LeagueCategory = "Others 1",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Winner",
                IsSaba = false,
                LeagueCategory = "Others 2",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Last corners",
                IsSaba = false,
                LeagueCategory = "Others 2",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Last bookings",
                IsSaba = false,
                LeagueCategory = "Others 3",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Single Team Home/Away (2 events)",
                IsSaba = false,
                LeagueCategory = "Others 4",
                MatchCount = 4
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "1st Penalty To Score",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "1st Penalty Awarded",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Next Goal kick",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Last Goal kick",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Goal kick (2H 1st xxx)",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Offside (2H 1st xxx)",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Last Throw In",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Throw In (2H 1st xxx)",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Last Free kick",
                IsSaba = false,
                LeagueCategory = "Others 6",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Free kick (2H 1st xxx)",
                IsSaba = false,
                LeagueCategory = "Unknown",
                MatchCount = 1
            }
            ,
            new()
            {
                EventDate = eventDate,
                BetType = "Special goal",
                IsSaba = false,
                LeagueCategory = "Unknown",
                MatchCount = 1
            }
            ,
            new()
            {
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