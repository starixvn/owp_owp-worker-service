using OWP.SumReportData.MatchLiveReport.Constants;
using OWP.SumReportData.MatchLiveReport.Helpers;
using OWP.SumReportData.MatchLiveReport.Models;
using OWP.SumReportData.MatchLiveReport.Services;
using NSubstitute;
using Xunit;
using Xunit.Asserts.Compare;
using OWP.SumReportData.MatchLiveReport.Repository;

namespace OWP.SumReportData.UnitTests.MatchLiveReport.Helpers;

public class ReportServiceUnitTests
{
    [Fact]
    public async Task Calculate_WithEmptyReport_ShouldNotInsertData()
    {
        var reportRepository = Substitute.For<ReportRepository>();
        reportRepository.QueryMatchesByDate(Arg.Any<DateTime>()).Returns(new List<OWPMatch>());
        var service = new ReportServiceImp(reportRepository);

        await service.Calculate(DateTime.Today);

        await reportRepository.DidNotReceive().InsertData(Arg.Any<IEnumerable<ReportModel>>(), Arg.Any<DateTime>());
    }

    [Fact]
    public async Task Calculate_WithNotEmptyReport_ShouldInsertData()
    {
        var reportRepository = Substitute.For<ReportRepository>();
        var matches = new List<OWPMatch> {
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15Mins,
                LeagueCategory = "MJ"
            },
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific10Mins,
                LeagueCategory = "MJ"
            }
        };
        reportRepository.QueryMatchesByDate(Arg.Any<DateTime>()).Returns(matches);
        var service = new ReportServiceImp(reportRepository);

        await service.Calculate(DateTime.Today);

        await reportRepository.Received().InsertData(Arg.Any<IEnumerable<ReportModel>>(), Arg.Any<DateTime>());
    }

    [Fact]
    public void Calculate_WithMatches_ShouldReturnReportModel()
    {
        var reportRepository = Substitute.For<ReportRepository>();
        reportRepository.QueryMatchesByDate(Arg.Any<DateTime>()).Returns(new List<OWPMatch>());
        var service = new ReportServiceImp(reportRepository);
        var eventDate = DateTime.Today;

        var matches = new List<OWPMatch>
        {
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15Mins,
                LeagueCategory = "MJ"
            },
            new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific10Mins,
                LeagueCategory = "MJ"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Corners,
                LeagueCategory = "MJ"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextCorners,
                LeagueCategory = "MJ"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15MinsNumberOfCorners,
                LeagueCategory = "M1"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Bookings,
                LeagueCategory = "M1"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextBookings,
                LeagueCategory = "M1"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Specific15MinsTotalBookings,
                LeagueCategory = "M1"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.WhichTeamToTakeTheFirstPenalty,
                LeagueCategory = "Others 1"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Winner,
                LeagueCategory = "Others 2"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastCorners,
                LeagueCategory = "Others 2"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastBookings,
                LeagueCategory = "Others 3"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOU,
                LeagueCategory = "Others 4"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOU,
                LeagueCategory = "Others 4"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOU,
                LeagueCategory = "Others 4"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FirstPenaltyToScore,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FirstPenaltyAwarded,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.NextGoalKick,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastGoalKick,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.GoalKick,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.Offside,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastThrowIn,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.ThrowIn,
                LeagueCategory = "Others 6"
            }
            ,new OWPMatch{
                MinorBettypeId = (int)MinorBetType.LastFreeKick,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.FreeKick,
                LeagueCategory = "Unknown"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.SpecialGoal,
                LeagueCategory = "Unknown"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                LeagueCategory = "Others 6"
            },new OWPMatch{
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                LeagueCategory = "Others 6"
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC FIFA - 16 Mins ",
                LeagueCategory = "Others 6"
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC FIFA - 6 mins ",
                LeagueCategory = "Others 5"
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "M88 SABA ABC FIFA - 16 mins ",
                LeagueCategory = "Others 4"
            },
            new OWPMatch{
                IsSaba = true,
                OWPLeagueName = "SABA ABC PES - 15 Mins ",
                LeagueCategory = "Others 3"
            }
            ,
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "abc fantasy ",
                LeagueCategory = "MJ"
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL LEAGUE MITTE",
                LeagueCategory = "MJ"
            },
            new OWPMatch{
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL WHICH TEAM TO KICK OFF",
                LeagueCategory = "MJ"
            },
        };

        var expected = new List<ReportModel> {
            new ReportModel {
                EventDate = eventDate,
                BetType = "Fantasy Match",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
            new ReportModel {
                EventDate = eventDate,
                BetType = "Main",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
            new ReportModel {
                EventDate = eventDate,
                BetType = "Which team to kick off (ET)",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
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
                BetType = "Corners",
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
                MatchCount = 2
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
                MatchCount = 2
            },

            new ReportModel {
                EventDate = eventDate,
                BetType = "SABA CLUB FRIENDLY Virtual FIFA 21 - 16 Mins Play",
                IsSaba = true,
                LeagueCategory = "Others 6",
                MatchCount = 1
            },
            new ReportModel {
                EventDate = eventDate,
                BetType = "SABA CLUB FRIENDLY Virtual FIFA 20 FUTSAL - 6 Mins Play",
                IsSaba = true,
                LeagueCategory = "Others 5",
                MatchCount = 1
            },
            new ReportModel {
                EventDate = eventDate,
                BetType = "M88 SABA SPANISH LEAGUE Virtual FIFA 21 Season 10 - 16 Mins Play",
                IsSaba = true,
                LeagueCategory = "Others 4",
                MatchCount = 1
            },
            new ReportModel {
                EventDate = eventDate,
                BetType = "SABA CLUB FRIENDLY Virtual PES 21 - 15 Mins Play",
                IsSaba = true,
                LeagueCategory = "Others 3",
                MatchCount = 1
            }
        };

        DeepAssert.Equal(expected, service.Calculate(eventDate, matches).ToList());
    }
}