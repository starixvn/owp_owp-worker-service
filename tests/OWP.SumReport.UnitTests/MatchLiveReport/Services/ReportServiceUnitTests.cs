namespace OWP.SumReport.UnitTests.MatchLiveReport.Helpers;

using NSubstitute;
using SumReport.MatchLiveReport.Constants;
using SumReport.MatchLiveReport.Helpers;
using SumReport.MatchLiveReport.Models;
using SumReport.MatchLiveReport.Repository;
using SumReport.MatchLiveReport.Services;
using Xunit;
using Xunit.Asserts.Compare;

public class ReportServiceUnitTests
{
    [Fact]
    public async Task Calculate_WithNotEmptyReport_ShouldInsertData()
    {
        var reportRepository = Substitute.For<ReportRepository>();
        var matches = new List<OWPMatch> {
            new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15Mins,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            },
            new()
            {
                MinorBettypeId = (int)MinorBetType.Specific10Mins,
                LeagueCategory = "MJ",
                ParentMatchId = 1
            }
        };
        var mainMatches = new List<OWPMatch> {
            new()
            {
                LeagueCategory = "MJ",
                OWPMatchId = 1
            },
            new()
            {
                LeagueCategory = "MJ",
                OWPMatchId = 2
            }
        };
        var mockSaba = new List<int> { 1, 3, 4 };
        reportRepository.QueryMatchesByDate(Arg.Any<DateTime>(), false).Returns((matches, mainMatches));
        reportRepository.GetSaba5minOWPMatchId(Arg.Any<List<int>>()).Returns(mockSaba);
        var service = new ReportServiceImp(reportRepository);

        await service.Calculate(DateTime.Today);

        await reportRepository.Received().InsertData(Arg.Any<IEnumerable<ReportModel>>(), Arg.Any<DateTime>());
    }

    [Fact]
    public void Calculate_WithMatches_ShouldReturnReportModel()
    {
        var reportRepository = Substitute.For<ReportRepository>();
        reportRepository.QueryMatchesByDate(Arg.Any<DateTime>(), false).Returns((new List<OWPMatch>(), new List<OWPMatch>()));
        var service = new ReportServiceImp(reportRepository);
        var eventDate = DateTime.Today;

        var mainMatches = new List<OWPMatch>
        {
            new()
            {
                OWPMatchId = 1,
                LeagueCategory = "MJ"
            },
            new()
            {
                OWPMatchId = 2,
                LeagueCategory = "M1"
            },
            new()
            {
                OWPMatchId = 3,
                LeagueCategory = "Others 1"
            },
            new()
            {
                OWPMatchId = 4,
                LeagueCategory = "Others 2"
            },
            new()
            {
                OWPMatchId = 5,
                LeagueCategory = "Others 3"
            },
            new()
            {
                OWPMatchId = 6,
                LeagueCategory = "Others 4"
            },
            new()
            {
                OWPMatchId = 7,
                LeagueCategory = "Others 5"
            },
            new()
            {
                OWPMatchId = 8,
                LeagueCategory = "Others 6"
            },
            new()
            {
                OWPMatchId = 9,
                LeagueCategory = "Unknown"
            }
        };

        var matches = new List<OWPMatch>
        {
            new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15Mins,
                ParentMatchId = 1,
                LeagueCategory = "MJ"
            },
            new()
            {
                MinorBettypeId = (int)MinorBetType.Specific10Mins,
                ParentMatchId = 1,
                LeagueCategory = "MJ"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Corners,
                ParentMatchId = 1,
                LeagueCategory = "MJ"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextCorners,
                ParentMatchId = 1,
                LeagueCategory = "MJ"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15MinsNumberOfCorners,
                ParentMatchId = 2,
                LeagueCategory = "M1"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Bookings,
                ParentMatchId = 2,
                LeagueCategory = "M1"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextBookings,
                ParentMatchId = 2,
                LeagueCategory = "M1"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Specific15MinsTotalBookings,
                ParentMatchId = 2,
                LeagueCategory = "M1"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.WhichTeamToTakeTheFirstPenalty,
                ParentMatchId = 3,
                LeagueCategory = "Others 1"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Winner,
                ParentMatchId = 4,
                LeagueCategory = "Others 2"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastCorners,
                ParentMatchId = 4,
                LeagueCategory = "Others 2"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastBookings,
                ParentMatchId = 5,
                LeagueCategory = "Others 3"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOU,
                ParentMatchId = 6,
                LeagueCategory = "Others 4"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwaySingleTeamOU,
                ParentMatchId = 6,
                LeagueCategory = "Others 4"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomeSingleTeamOU,
                ParentMatchId = 6,
                LeagueCategory = "Others 4"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FirstPenaltyToScore,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FirstPenaltyAwarded,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.NextGoalKick,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastGoalKick,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.GoalKick,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.Offside,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.LastThrowIn,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.ThrowIn,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            }
            ,new()
            {
                MinorBettypeId = (int)MinorBetType.LastFreeKick,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.FreeKick,
                ParentMatchId = 9,
                LeagueCategory = "Unknown"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.SpecialGoal,
                ParentMatchId = 9,
                LeagueCategory = "Unknown"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.AwayPenaltyShootOut,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },new()
            {
                MinorBettypeId = (int)MinorBetType.HomePenaltyShootOut,
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },
            new()
            {
                IsSaba = true,
                OWPLeagueName = "SABA ABC FIFA - 16 Mins ",
                ParentMatchId = 8,
                LeagueCategory = "Others 6"
            },
            new()
            {
                IsSaba = true,
                OWPLeagueName = "SABA ABC FIFA - 6 mins ",
                ParentMatchId = 7,
                LeagueCategory = "Others 5"
            },
            new()
            {
                IsSaba = true,
                OWPLeagueName = "M88 SABA ABC FIFA - 16 mins ",
                ParentMatchId = 6,
                LeagueCategory = "Others 4"
            },
            new()
            {
                IsSaba = true,
                OWPLeagueName = "SABA ABC PES - 15 Mins ",
                ParentMatchId = 5,
                LeagueCategory = "Others 3"
            }
            ,
            new()
            {
                IsSaba = false,
                OWPLeagueName = "abc fantasy ",
                ParentMatchId = 1,
                LeagueCategory = "MJ"
            },
            new()
            {
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL LEAGUE MITTE",
                ParentMatchId = 1,
                LeagueCategory = "MJ"
            },
            new()
            {
                IsSaba = false,
                OWPLeagueName = "AUSTRIA REGIONAL WHICH TEAM TO KICK OFF",
                ParentMatchId = 1,
                LeagueCategory = "MJ"
            },
        };

        var minorMatchesCategorized = MatchHelper.CategorizeMinorMatches(matches, mainMatches);
        var mainMatchesCategorized = MatchHelper.CategorizeMainMatches(matches);
        var sabaMatchesCategorized = MatchHelper.CategorizeSabaMatches(matches);

        var expected = new List<ReportModel> {
            new()
            {
                EventDate = eventDate,
                BetType = "Fantasy Match",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
            new()
            {
                EventDate = eventDate,
                BetType = "Main",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
            new()
            {
                EventDate = eventDate,
                BetType = "Which team to kick off (ET)",
                IsSaba = false,
                LeagueCategory = "MJ",
                MatchCount = 1
            },
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
                MatchCount = 2
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
                MatchCount = 2
            },

            new()
            {
                EventDate = eventDate,
                BetType = "SABA CLUB FRIENDLY Virtual FIFA 21 - 16 Mins Play",
                IsSaba = true,
                LeagueCategory = "Others 6",
                MatchCount = 1
            },
            new()
            {
                EventDate = eventDate,
                BetType = "SABA CLUB FRIENDLY Virtual FIFA 20 FUTSAL - 6 Mins Play",
                IsSaba = true,
                LeagueCategory = "Others 5",
                MatchCount = 1
            },
            new()
            {
                EventDate = eventDate,
                BetType = "M88 SABA SPANISH LEAGUE Virtual FIFA 21 Season 10 - 16 Mins Play",
                IsSaba = true,
                LeagueCategory = "Others 4",
                MatchCount = 1
            },
            new()
            {
                EventDate = eventDate,
                BetType = "SABA CLUB FRIENDLY Virtual PES 21 - 15 Mins Play",
                IsSaba = true,
                LeagueCategory = "Others 3",
                MatchCount = 1
            }
        };

        DeepAssert.Equal(expected, service.Calculate(eventDate, minorMatchesCategorized, mainMatchesCategorized, sabaMatchesCategorized).ToList());
    }
}