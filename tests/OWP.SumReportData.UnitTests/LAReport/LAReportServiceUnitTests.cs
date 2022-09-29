using NSubstitute;
using OWP.SumReportData.LAReport;
using OWP.SumReportData.LAReport.Enum;
using OWP.SumReportData.LAReport.Factory;
using OWP.SumReportData.LAReport.Misc;
using OWP.SumReportData.LAReport.Models;
using OWP.SumReportData.LAReport.Services;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.SumReportData.UnitTests.LAReport;

public class LAReportServiceUnitTests
{
    [Fact]
    public async Task SearchLAReport_ShouldReturnData()
    {
        //Arrange
        var calculationService = CreateCalculationService();

        //Act
        var actual = (await calculationService.Calculate(DateTime.Today, false)).ToList();

        //Assert
        var expected = CreateExpectedLAReportModel();
        DeepAssert.Equal(expected, actual);
    }

    private CalculationService CreateCalculationService()
    {
        var assignments = CreateFakeAssignments();
        var sections = CreateFakeUserSections();
        var queryRepository = Substitute.For<LARepository>();
        queryRepository.GetLAAssignment(Arg.Compat.Any<DateTime>(), false).Returns(assignments);
        queryRepository.GetUserSection().Returns(sections);

        var specialProductFactory = Substitute.For<SpecialProductFactory>();
        specialProductFactory.Create(RuleType.NOT_CORNER).Returns(new NotCornerRule());
        specialProductFactory.Create(RuleType.CORNER).Returns(new IsCornerRule());

        var mainMatchRuleFactory = Substitute.For<MainMatchRuleFactory>();
        mainMatchRuleFactory.Create(RuleType.FT_CORNER).Returns(new FTCornerRule());
        mainMatchRuleFactory.Create(RuleType.FANTASY).Returns(new IsFantasyRule());
        mainMatchRuleFactory.Create(RuleType.ESOCCER).Returns(new ESoccerRule());
        mainMatchRuleFactory.Create(RuleType.FIVE_MAJOR_LEAGUE).Returns(new FiveMajorLeagueRule());
        mainMatchRuleFactory.Create(RuleType.FT_CORNER_FIVE_MAJOR_LEAGUE).Returns(new FTCorner5MLRule());

        var multipleAndSingleMatchesFactory = Substitute.For<SingleMatchesFactory>();
        multipleAndSingleMatchesFactory.Create(RuleType.SINGLE).Returns(new IsSingleMatchesRule());

        var calculationService = new CalculationServiceImp(queryRepository, specialProductFactory, mainMatchRuleFactory, multipleAndSingleMatchesFactory);
        return calculationService;
    }

    private static List<Assignment> CreateFakeAssignments()
    {
        return new List<Assignment>
            {
                //T.Booking+
                new Assignment
                {
                    AssignmentId = 1,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "leaguemar27",
                    MinorBettypeId = -6,
                    OWPMatchId = 428030,
                    ParentMatchId = 428020,
                    EventDateTime = new DateTime(2020, 7, 1, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 2,
                    UserId = 15477090,
                    ProviderName = "BET",
                    OWPLeagueName = "leaguemar27 - CORNERS",
                    MinorBettypeId = -6,
                    OWPMatchId = 428035,
                    ParentMatchId = 428031,
                    EventDateTime = new DateTime(2020, 7, 1, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 3,
                    UserId = 15477090,
                    ProviderName = "MRB",
                    OWPLeagueName = "leaguemar28",
                    MinorBettypeId = -7,
                    OWPMatchId = 428029,
                    ParentMatchId = 428031,
                    EventDateTime = new DateTime(2020, 7, 1, 15, 0, 0)
                },
                //Goal Kick
                new Assignment
                {
                    AssignmentId = 4,
                    UserId = 15477090,
                    ProviderName = "FC",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERS",
                    MinorBettypeId = -100,
                    OWPMatchId = 425385,
                    ParentMatchId = 123456,
                    EventDateTime = new DateTime(2020, 7, 2, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 5,
                    UserId = 15477090,
                    ProviderName = "TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS",
                    MinorBettypeId = -101,
                    OWPMatchId = 425386,
                    ParentMatchId = 123456,
                    EventDateTime = new DateTime(2020, 7, 2, 15, 0, 0)
                },
                //Other Products
                new Assignment
                {
                    AssignmentId = 6,
                    UserId = 15477090,
                    ProviderName = "Reporter",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS",
                    MinorBettypeId = -1,
                    OWPMatchId = 425387,
                    ParentMatchId = 123456,
                    EventDateTime = new DateTime(2020, 7, 2, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 7,
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS",
                    MinorBettypeId = -109,
                    OWPMatchId = 425388,
                    ParentMatchId = 123456,
                    EventDateTime = new DateTime(2020, 7, 2, 15, 0, 0)
                },
                //FT_Corner
                new Assignment
                {
                    AssignmentId = 8,
                    UserId = 15477090,
                    ProviderName = "Reporter",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS",
                    MinorBettypeId = -3,
                    OWPMatchId = 425387,
                    ParentMatchId = 123456,
                    EventDateTime = new DateTime(2020, 7, 3, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 9,
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS",
                    MinorBettypeId = -4,
                    OWPMatchId = 425388,
                    ParentMatchId = 123456,
                    EventDateTime = new DateTime(2020, 7, 3, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 10,
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIER",
                    MinorBettypeId = null,
                    OWPMatchId = 123456,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 3, 15, 0, 0)
                },
                //Corner+
                new Assignment
                {
                    AssignmentId = 11,
                    UserId = 15477090,
                    ProviderName = "Reporter",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS",
                    MinorBettypeId = -3,
                    OWPMatchId = 425001,
                    ParentMatchId = 425000,
                    EventDateTime = new DateTime(2020, 7, 4, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 12,
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERSy",
                    MinorBettypeId = -4,
                    OWPMatchId = 425002,
                    ParentMatchId = 425000,
                    EventDateTime = new DateTime(2020, 7, 4, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 13,
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS -  play",
                    MinorBettypeId = -5,
                    OWPMatchId = 425004,
                    ParentMatchId = 425003,
                    EventDateTime = new DateTime(2020, 7, 4, 15, 0, 0)
                },

                //Fantasy
                new Assignment
                {
                    AssignmentId = 14,
                    UserId = 15477090,
                    ProviderName = "Reporter",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS FANTASY MATCH",
                    MinorBettypeId = null,
                    OWPMatchId = 425005,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 5, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 15,
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS FANTASY MATCH",
                    MinorBettypeId = null,
                    OWPMatchId = 425006,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 5, 15, 0, 0)
                },
                //ESOCCER
                new Assignment
                {
                    AssignmentId = 16,
                    UserId = 15477090,
                    ProviderName = "Reporter",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - 5 Mins Play",
                    MinorBettypeId = null,
                    OWPMatchId = 425007,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 6, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 16,
                    UserId = 15477090,
                    ProviderName = "Reporter",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - 6 Mins Play",
                    MinorBettypeId = null,
                    OWPMatchId = 425112,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 6, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 16,
                    UserId = 15477090,
                    ProviderName = "Reporter",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - 6 Mins Play",
                    MinorBettypeId = null,
                    OWPMatchId = 425113,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 6, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 17,
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - 15 Mins Play",
                    MinorBettypeId = null,
                    OWPMatchId = 425008,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 6, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 17,
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OWPLeagueName = "XSABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS-test- 16 Mins Play",
                    MinorBettypeId = null,
                    OWPMatchId = 425008,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2021, 09, 06, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 17,
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS-test- 20 Mins Play",
                    MinorBettypeId = null,
                    OWPMatchId = 425008,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2021, 09, 06, 15, 0, 0)
                },

                //MULTIPLE MATCHES
                new Assignment
                {
                    AssignmentId = 18,
                    UserId = 15477090,
                    ProviderName = "RB2/IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERSPlay",
                    MinorBettypeId = null,
                    OWPMatchId = 425009,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 7, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 19,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 425010,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 7, 15, 15, 0)
                },
                new Assignment
                {
                    AssignmentId = 20,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 425012,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 7, 16, 05, 0)
                },
                new Assignment
                {
                    AssignmentId = 21,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 425013,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 7, 16, 8, 0)
                },
                //X3
                new Assignment
                {
                    AssignmentId = 22,
                    UserId = 15477090,
                    ProviderName = "RB2/IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERSPlay",
                    MinorBettypeId = null,
                    OWPMatchId = 425014,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 8, 16, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 23,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 425015,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 8, 16, 1, 0)
                },
                new Assignment
                {
                    AssignmentId = 24,
                    UserId = 15477090,
                    ProviderName = "RB2/IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERSPlay",
                    MinorBettypeId = null,
                    OWPMatchId = 425016,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 8, 16, 3, 0)
                },
                new Assignment
                {
                    AssignmentId = 25,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 425017,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 8, 16, 05, 0)
                },
                new Assignment
                {
                    AssignmentId = 26,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 425018,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 8, 16, 8, 0)
                },
                //X6
                new Assignment
                {
                    AssignmentId = 27,
                    UserId = 15477090,
                    ProviderName = "RB2/IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERSPlay",
                    MinorBettypeId = null,
                    OWPMatchId = 425035,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 20, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 28,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 425036,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 20, 1, 0)
                },
                new Assignment
                {
                    AssignmentId = 29,
                    UserId = 15477090,
                    ProviderName = "RB2/IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERSPlay",
                    MinorBettypeId = null,
                    OWPMatchId = 425037,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 20, 3, 0)
                },
                new Assignment
                {
                    AssignmentId = 30,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 425038,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 20, 05, 0)
                },
                new Assignment
                {
                    AssignmentId = 31,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 425039,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 20, 8, 0)
                },
                new Assignment
                {
                    AssignmentId = 32,
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 425040,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 20, 12, 0)
                },
                //SINGLE MATCHES
                new Assignment
                {
                    AssignmentId = 33,
                    UserId = 15477090,
                    ProviderName = "RB2/TV", // Mixed
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERSPlay",
                    MinorBettypeId = null,
                    OWPMatchId = 424035,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 9, 1, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 34,
                    UserId = 15477090,
                    ProviderName = "RTS", // Only Provider
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 424036,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 9, 2, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 35,
                    UserId = 15477090,
                    ProviderName = "BET365(TV)", // Only TV
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERSPlay",
                    MinorBettypeId = null,
                    OWPMatchId = 445037,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 9, 3, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 36,
                    UserId = 15477090,
                    ProviderName = "Waiting AL", // Only Reporter
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALay",
                    MinorBettypeId = null,
                    OWPMatchId = 424038,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 9, 4, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 37,
                    UserId = 15477090,
                    ProviderName = "Reporter", // Only reporter
                    OWPLeagueName = "Futsal", // Only TV
                    MinorBettypeId = null,
                    OWPMatchId = 424039,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 9, 5, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 38,
                    UserId = 15477090,
                    ProviderName = null,
                    OWPLeagueName = "Futsal ABC", // Only TV
                    MinorBettypeId = null,
                    OWPMatchId = 424040,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 9, 6, 0, 0)
                },
                //OTHERS
                new Assignment
                {
                    AssignmentId = 110,
                    UserId = 15477090,
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERSy",
                    MinorBettypeId = null,
                    OWPMatchId = 425002,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 4, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 130,
                    UserId = 15477090,
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS -  play",
                    MinorBettypeId = null,
                    OWPMatchId = 425004,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 4, 15, 0, 0)
                },
                //5ML
                new Assignment
                {
                    AssignmentId = 131,
                    UserId = 15477090,
                    OWPLeagueName = "*ENGLISH PREMIER LEAGUE",
                    MinorBettypeId = null,
                    OWPMatchId = 546764,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2022, 4, 9, 15, 0, 0)
                },
                //FT_Corner_5ML
                new Assignment
                {
                    AssignmentId = 131,
                    UserId = 15477090,
                    OWPLeagueName = "*ENGLISH PREMIER LEAGUE",
                    MinorBettypeId = null,
                    OWPMatchId = 425998,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2022, 4, 9, 15, 0, 0)
                },
                new Assignment
                {
                    AssignmentId = 131,
                    UserId = 15477090,
                    OWPLeagueName = "*ENGLISH PREMIER LEAGUE - CORNERS",
                    MinorBettypeId = -4,
                    OWPMatchId = 427128,
                    ParentMatchId = 425998,
                    EventDateTime = new DateTime(2022, 4, 9, 15, 0, 0)
                }
            };
    }

    private static Dictionary<int, string> CreateFakeUserSections()
    {
        return new Dictionary<int, string>
            {
                { 15477090, "sa"}
            };
    }

    private static IEnumerable<LAReportModel> CreateExpectedLAReportModel()
    {
        return new List<LAReportModel>
            {
                new LAReportModel
                {
                    UserId = 15477090,
                    Username = "sa",
                    TBookingPlus = 2,
                    GoalKick = 1,
                    OtherProducts = 2,
                    FTCorner = 1,
                    CornerPlus = 2,
                    Fantasy = 2,
                    ESoccer15Mins = 1,
                    ESoccer5Mins = 1,
                    ESoccer6Mins = 2,
                    ESoccer16Mins = 1,
                    ESoccer20Mins = 1,
                    Link2 = 3,
                    Link3 = 0,
                    Link4 = 0,
                    Link5 = 1,
                    Link6 = 1,
                    Link7 = 0,
                    Link8Plus = 0,
                    MixedProviderTVReporter = 1,
                    OnlyProvider = 1,
                    OnlyTV = 2,
                    OnlyReporter = 0,
                    Others = 2,
                    FiveMajorLeague = 1,
                    FTCorner5ML = 1,
                    OtherProductMatchIds = "424038,424040"
                }
            };
    }
}