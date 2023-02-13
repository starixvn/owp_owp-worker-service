namespace OWP.SumReport.UnitTests.TraderReport;

using SumReport.TraderReport.Models;
using SumReport.TraderReport.Services;
using Xunit;

public class RobotX3ServiceUnitTests
{
    private readonly RobotX3Service robotX3Service;

    public RobotX3ServiceUnitTests()
    {
        robotX3Service = new RobotX3ServiceImp();
    }

    [Fact]
    public void GroupConsecutiveMatch_AssignmentEventDateTimeOver30_DifferentGroup()
    {
        var model1 = new RobotX3RawModel
        {
            EventDateTime = DateTime.Parse("03/16/2020 2:00:00 PM"),
            OwpMatchId = 406897,
            UserId = 8
        };

        var model2 = new RobotX3RawModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 11:01:00 AM"),
            OwpMatchId = 406913,
            UserId = 8
        };

        var model3 = new RobotX3RawModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 1:00:00 PM"),
            OwpMatchId = 406934,
            UserId = 8
        };

        var model4 = new RobotX3RawModel
        {
            EventDateTime = DateTime.Parse("03/18/2020 10:00:00 AM"),
            OwpMatchId = 406950,
            UserId = 8
        };

        var model5 = new RobotX3RawModel
        {
            EventDateTime = DateTime.Parse("03/27/2020 2:00:00 PM"),
            OwpMatchId = 406987,
            UserId = 8
        };

        var model6 = new RobotX3RawGroupModel
        {
            EventDateTime = DateTime.Parse("03/16/2020 2:00:00 PM"),
            MatchGroup = 1,
            OwpMatchId = 406897,
            UserId = 8
        };

        var model7 = new RobotX3RawGroupModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 11:01:00 AM"),
            MatchGroup = 2,
            OwpMatchId = 406913,
            UserId = 8
        };

        var model8 = new RobotX3RawGroupModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 1:00:00 PM"),
            MatchGroup = 3,
            OwpMatchId = 406934,
            UserId = 8
        };

        var model9 = new RobotX3RawGroupModel
        {
            EventDateTime = DateTime.Parse("03/18/2020 10:00:00 AM"),
            MatchGroup = 4,
            OwpMatchId = 406950,
            UserId = 8
        };

        var model10 = new RobotX3RawGroupModel
        {
            EventDateTime = DateTime.Parse("03/27/2020 2:00:00 PM"),
            MatchGroup = 5,
            OwpMatchId = 406987,
            UserId = 8
        };

        var assignmentList = new List<RobotX3RawModel> { model1, model2, model3, model4, model5 };
        var expected = new List<RobotX3RawGroupModel> { model6, model7, model8, model9, model10 };
        var actual = robotX3Service.GroupConsecutiveMatch(assignmentList);
        for (var i = 0; i < actual.Count; i++)
        {
            Assert.True(actual[i].MatchGroup.Equals(expected[i].MatchGroup));
        }
    }

    [Fact]
    public void GroupConsecutiveMatch_AssignmentEventDateTimeWithin30_TogetherGroup()
    {
        var assignmentList = new List<RobotX3RawModel>
            {
                 new()
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:30:00 PM"),
                      OwpMatchId = 347272,
                      UserId = 17013702
                 },
                 new()
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:45:00 PM"),
                      OwpMatchId = 347277,
                      UserId = 17013702
                 }
            };

        var expected = new List<RobotX3RawGroupModel>
            {
                 new()
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:30:00 PM"),
                      MatchGroup = 1,
                      OwpMatchId = 347272,
                      UserId = 17013702
                 },
                 new()
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:45:00 PM"),
                      MatchGroup = 1,
                      OwpMatchId = 347277,
                      UserId = 17013702
                 }
            };

        var actual = robotX3Service.GroupConsecutiveMatch(assignmentList);
        for (var i = 0; i < actual.Count; i++)
        {
            Assert.True(actual[i].MatchGroup.Equals(expected[i].MatchGroup));
        }
    }

    [Fact]
    public void GetTraderAssignments_Successful()
    {
        var assignmentList = new List<RobotX3RawModel>
            {
                 new()
                 {
                      EventDateTime = DateTime.Parse("03/16/2020 2:00:00 PM"),
                      OwpMatchId = 406897,
                      UserId = 8
                 },
                 new()
                 {
                      EventDateTime = DateTime.Parse("03/17/2020 11:01:00 AM"),
                      OwpMatchId = 406913,
                      UserId = 8
                 },
                 new()
                 {
                      EventDateTime = DateTime.Parse("03/17/2020 1:00:00 PM"),
                      OwpMatchId = 406934,
                      UserId = 8
                 },
                 new()
                 {
                      EventDateTime = DateTime.Parse("03/18/2020 10:00:00 AM"),
                      OwpMatchId = 406950,
                      UserId = 8
                 },
                  new()
                  {
                      EventDateTime = DateTime.Parse("03/27/2020 2:00:00 PM"),
                      OwpMatchId = 406987,
                      UserId = 8
                 },
                  new()
                  {
                      EventDateTime = DateTime.Parse("04/02/2020 1:30:00 PM"),
                      OwpMatchId = 347272,
                      UserId = 17013702
                 },
                 new()
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:45:00 PM"),
                      OwpMatchId = 347277,
                      UserId = 17013702
                 }
            };

        var expected = new List<RobotX3ByUser>
            {
                new()
                {
                    UserId = 8,
                    X2 = 0,
                    X3 = 0,
                    X3Plus = 0
                },
                new()
                {
                    UserId = 17013702,
                    X2 = 1,
                    X3 = 0,
                    X3Plus = 0
                }
            };

        var actual = robotX3Service.Aggregate(assignmentList);
        for (var i = 0; i < actual.Count; i++)
        {
            Assert.True(actual[i].X2.Equals(expected[i].X2));
            Assert.True(actual[i].X3.Equals(expected[i].X3));
            Assert.True(actual[i].X3Plus.Equals(expected[i].X3Plus));
        }
    }
}