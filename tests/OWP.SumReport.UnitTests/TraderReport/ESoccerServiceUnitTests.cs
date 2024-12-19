namespace OWP.SumReport.UnitTests.TraderReport;

using NSubstitute;
using SumReport.TraderReport;
using SumReport.TraderReport.Models;
using SumReport.TraderReport.Services;
using Xunit;
using Xunit.Asserts.Compare;

public class ESoccerServiceUnitTests
{
    private readonly ESoccerServiceImp service;

    private readonly TraderRepository repository;

    public ESoccerServiceUnitTests()
    {
        repository = Substitute.For<TraderRepository>();
        service = new ESoccerServiceImp(repository);
    }

    [Fact]
    public async Task SumESoccerFields()
    {
        repository.GetSaBaAssignmentByDate(Arg.Any<DateTime>(), false).Returns(MockAssignments());

        var result = await service.SumESoccerFields(new DateTime(2022, 1, 20), false);

        DeepAssert.Equal(MockExpect(), result.ToList());
    }

    private static List<ESoccerByUser> MockExpect()
        =>
        [
            new()
            {
                UserId = 111,
                ESoccer5Mins = 1,
                ESoccer6Mins = 1,
                ESoccer15Mins = 2,
                ESoccer16Mins = 1,
                ESoccer20Mins = 2
            },

            new()
            {
                UserId = 222,
                ESoccer5Mins = 1,
                ESoccer6Mins = 1,
                ESoccer15Mins = 1,
                ESoccer16Mins = 0,
                ESoccer20Mins = 0
            }
        ];

    private static List<Assignment> MockAssignments()
        =>
        [
            new() { UserId = 111, OwpMatchId = 1, OwpLeagueName = "SABA - 5 Mins Play" },
            new() { UserId = 111, OwpMatchId = 2, OwpLeagueName = "SABA - 6 Mins Play" },
            new() { UserId = 111, OwpMatchId = 3, OwpLeagueName = "SABA - 15 Mins Play" },
            new() { UserId = 111, OwpMatchId = 4, OwpLeagueName = "SABA - 15 Mins Play" },
            new() { UserId = 111, OwpMatchId = 5, OwpLeagueName = "SABA - 16 Mins Play" },
            new() { UserId = 111, OwpMatchId = 6, OwpLeagueName = "SABA - 20 Mins Play" },
            new() { UserId = 111, OwpMatchId = 7, OwpLeagueName = "SABA - 20 Mins Play" },
            new() { UserId = 111, OwpMatchId = 7, OwpLeagueName = "SABA - 20 Mins Play" },
            new() { UserId = 111, OwpMatchId = 9, OwpLeagueName = "SABA" },
            new() { UserId = 222, OwpMatchId = 10, OwpLeagueName = "SABA - 5 Mins Play" },
            new() { UserId = 222, OwpMatchId = 11, OwpLeagueName = "SABA - 6 Mins Play" },
            new() { UserId = 222, OwpMatchId = 12, OwpLeagueName = "SABA - 15 Mins Play" }
        ];
}