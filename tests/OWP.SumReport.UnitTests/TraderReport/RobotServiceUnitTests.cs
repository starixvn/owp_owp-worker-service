namespace OWP.SumReport.UnitTests.TraderReport;

using NSubstitute;
using SumReport.TraderReport;
using SumReport.TraderReport.Models;
using SumReport.TraderReport.Services;
using Xunit;
using Xunit.Asserts.Compare;

public class RobotServiceUnitTests
{
    [Fact]
    public async Task SumRobotFields_AlwayReturnsDataForX3AndX5()
    {
        // Arrange
        var repository = Substitute.For<TraderRepository>();
        var robotX3Service = Substitute.For<RobotX3Service>();
        var robotX5Service = Substitute.For<RobotX5Service>();
        var service = new RobotServiceImp(repository, robotX3Service, robotX5Service);

        // Mock
        var x3FakeData = CreateX3FakeData();
        var x5FakeData = CreateX5FakeData();
        robotX3Service.Aggregate(Arg.Compat.Any<List<RobotX3RawModel>>()).Returns(x3FakeData);
        robotX5Service.Aggregate(Arg.Compat.Any<List<RobotX5RawModel>>()).Returns(x5FakeData);

        // Act
        var actual = await service.SumRobotFields(DateTime.Today, false);

        // Assert
        var expected = CreateExpectedData();
        DeepAssert.Equal(expected, actual);
    }

    private List<RobotX3ByUser> CreateX3FakeData()
    {
        var item1 = new RobotX3ByUser
        {
            UserId = 1,
            X2 = 10,
            X3 = 11,
            X3Plus = 12
        };

        var item2 = new RobotX3ByUser
        {
            UserId = 2,
            X2 = 20,
            X3 = 21,
            X3Plus = 22
        };

        var item3 = new RobotX3ByUser
        {
            UserId = 3,
            X2 = 30,
            X3 = 31,
            X3Plus = 32
        };

        return new List<RobotX3ByUser>
            {
                item1,
                item2,
                item3
            };
    }

    private List<RobotX5ByUser> CreateX5FakeData()
    {
        var item1 = new RobotX5ByUser
        {
            UserId = 2,
            X5 = 23
        };

        var item2 = new RobotX5ByUser
        {
            UserId = 3,
            X5 = 33
        };

        var item3 = new RobotX5ByUser
        {
            UserId = 4,
            X5 = 43
        };

        return new List<RobotX5ByUser>
            {
                item1,
                item2,
                item3
            };
    }

    private List<RobotByByUser> CreateExpectedData()
    {
        var item1 = new RobotByByUser
        {
            UserId = 1,
            X2 = 10,
            X3 = 11,
            X3Plus = 12,
            X5 = 0
        };

        var item2 = new RobotByByUser
        {
            UserId = 2,
            X2 = 20,
            X3 = 21,
            X3Plus = 22,
            X5 = 23
        };

        var item3 = new RobotByByUser
        {
            UserId = 3,
            X2 = 30,
            X3 = 31,
            X3Plus = 32,
            X5 = 33
        };

        var item4 = new RobotByByUser
        {
            UserId = 4,
            X2 = 0,
            X3 = 0,
            X3Plus = 0,
            X5 = 43
        };

        return new List<RobotByByUser>
            {
                item1,
                item2,
                item3,
                item4
            };
    }
}