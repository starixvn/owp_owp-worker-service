using OWP.SumReport.TraderReport.Models;
using OWP.SumReport.TraderReport.Services;
using Xunit;

namespace OWP.SumReport.UnitTests.TraderReport;

public class RobotX5ServiceUnitTests
{
    private readonly RobotX5Service robotX5Service;

    public RobotX5ServiceUnitTests()
    {
        robotX5Service = new RobotX5ServiceImp();
    }

    [Fact]
    public void GroupByShiftAndMatchGroup_AssigmentEventDateTimeOver30AndOver300OfTwoShift_DifferentGroupAndDifferentShift()
    {
        var model1 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 4:30:00 PM"),
            IsFullTime = false,
            OWPMatchId = 346949,
            UserId = 4635564
        };

        var model2 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 4:30:00 PM"),
            IsFullTime = false,
            OWPMatchId = 346949,
            UserId = 4635564
        };

        var model3 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 10:00:00 PM"),
            IsFullTime = false,
            OWPMatchId = 346954,
            UserId = 4635564
        };

        var model4 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 10:00:00 PM"),
            IsFullTime = false,
            OWPMatchId = 347145,
            UserId = 4635564
        };

        var model5 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 10:00:00 PM"),
            IsFullTime = false,
            OWPMatchId = 347152,
            UserId = 4635564
        };

        var model6 = new RobotX5RawGroupModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 4:30:00 PM"),
            OWPMatchId = 346949,
            UserId = 4635564,
            Minutes = 45,
            MatchGroup = 1,
            Shift = 1
        };

        var model7 = new RobotX5RawGroupModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 4:30:00 PM"),
            OWPMatchId = 346949,
            UserId = 4635564,
            Minutes = 45,
            MatchGroup = 1,
            Shift = 1
        };

        var model8 = new RobotX5RawGroupModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 10:00:00 PM"),
            OWPMatchId = 346954,
            UserId = 4635564,
            Minutes = 45,
            MatchGroup = 2,
            Shift = 2
        };

        var model9 = new RobotX5RawGroupModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 10:00:00 PM"),
            OWPMatchId = 347145,
            UserId = 4635564,
            Minutes = 45,
            MatchGroup = 2,
            Shift = 2
        };

        var model10 = new RobotX5RawGroupModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 10:00:00 PM"),
            OWPMatchId = 347152,
            UserId = 4635564,
            Minutes = 45,
            MatchGroup = 2,
            Shift = 2
        };

        var assigmentListOfUser = new List<RobotX5RawModel> { model1, model2, model3, model4, model5 };
        var expected = new List<RobotX5RawGroupModel> { model6, model7, model8, model9, model10 };
        var actual = robotX5Service.GroupByShiftAndMatchGroup(assigmentListOfUser);
        for (var i = 0; i < actual.Count; i++)
        {
            Assert.True(actual[i].MatchGroup.Equals(expected[i].MatchGroup));
            Assert.True(actual[i].Shift.Equals(expected[i].Shift));
        }
    }

    [Fact]
    public void GetTraderAssignments_Successful()
    {
        var model1 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 2:00:00 PM"),
            IsFullTime = false,
            OWPMatchId = 346921,
            UserId = 846029
        };

        var model2 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("04/01/2020 2:00:00 PM"),
            IsFullTime = true,
            OWPMatchId = 346921,
            UserId = 846029
        };

        var model3 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/16/2020 2:00:00 PM"),
            IsFullTime = true,
            OWPMatchId = 406897,
            UserId = 360418
        };

        var model4 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/16/2020 2:30:00 PM"),
            IsFullTime = true,
            OWPMatchId = 406902,
            UserId = 360418
        };

        var model5 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/16/2020 3:30:00 PM"),
            IsFullTime = false,
            OWPMatchId = 406907,
            UserId = 360418
        };

        var model6 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/16/2020 3:30:00 PM"),
            IsFullTime = true,
            OWPMatchId = 406903,
            UserId = 360418
        };

        var model7 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/16/2020 10:00:00 PM"),
            IsFullTime = false,
            OWPMatchId = 406904,
            UserId = 360418
        };

        var model8 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/16/2020 10:00:00 PM"),
            IsFullTime = true,
            OWPMatchId = 406905,
            UserId = 360418
        };

        var model9 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 10:00:00 AM"),
            IsFullTime = true,
            OWPMatchId = 406913,
            UserId = 360418
        };

        var model10 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 10:40:00 AM"),
            IsFullTime = true,
            OWPMatchId = 406918,
            UserId = 360418
        };

        var model11 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 11:30:00 AM"),
            IsFullTime = true,
            OWPMatchId = 406919,
            UserId = 360418
        };

        var model12 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 1:00:00 PM"),
            IsFullTime = true,
            OWPMatchId = 406920,
            UserId = 360418
        };

        var model13 = new RobotX5RawModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 2:00:00 PM"),
            IsFullTime = true,
            OWPMatchId = 406921,
            UserId = 360418
        };

        var assignmentList = new List<RobotX5RawModel>
            {
                model1,
                model2,
                model3,
                model4,
                model5,
                model6,
                model7,
                model8,
                model9,
                model10,
                model11,
                model12,
                model13
            };
        var expected = new List<RobotX5ByUser>
            {
                new RobotX5ByUser
                {
                    UserId = 846029,
                    X5 = 0
                },
                new RobotX5ByUser
                {
                    UserId = 360418,
                    X5 = 1
                }
            };

        var actual = robotX5Service.Aggregate(assignmentList);
        for (var i = 0; i < actual.Count; i++)
        {
            Assert.True(actual[i].X5.Equals(expected[i].X5));
        }
    }

    [Theory]
    [InlineData(true, true, 70)]
    [InlineData(true, false, 90)]
    [InlineData(false, true, 35)]
    [InlineData(false, false, 45)]
    public void GetMinuteOfMatch_ReturnsCorrectValue(bool isFullTime, bool is35MinuteMatch, int expected)
    {
        var actual = robotX5Service.GetMinuteOfMatch(isFullTime, is35MinuteMatch);

        Assert.Equal(expected, actual);
    }
}