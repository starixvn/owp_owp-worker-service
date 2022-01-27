using OWP.SumReportData.TraderReport.Models;
using OWP.SumReportData.TraderReport.Services;
using Xunit;

namespace OWP.SumReportData.UnitTests.TraderReport;

public class RobotX3ServiceUnitTests
{
    private readonly RobotX3Service robotX3Service;

    public RobotX3ServiceUnitTests()
    {
        robotX3Service = new RobotX3ServiceImp();
    }

    [Fact]
    public void GroupConecutiveMatch_AssigmentEventDateTimeOver30_DifferentGroup()
    {
        var model1 = new RobotX3RawModel
        {
            EventDateTime = DateTime.Parse("03/16/2020 2:00:00 PM"),
            OWPMatchId = 406897,
            UserId = 8
        };

        var model2 = new RobotX3RawModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 11:01:00 AM"),
            OWPMatchId = 406913,
            UserId = 8
        };

        var model3 = new RobotX3RawModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 1:00:00 PM"),
            OWPMatchId = 406934,
            UserId = 8
        };

        var model4 = new RobotX3RawModel
        {
            EventDateTime = DateTime.Parse("03/18/2020 10:00:00 AM"),
            OWPMatchId = 406950,
            UserId = 8
        };

        var model5 = new RobotX3RawModel
        {
            EventDateTime = DateTime.Parse("03/27/2020 2:00:00 PM"),
            OWPMatchId = 406987,
            UserId = 8
        };

        var model6 = new RobotX3RawGroupModel
        {
            EventDateTime = DateTime.Parse("03/16/2020 2:00:00 PM"),
            MatchGroup = 1,
            OWPMatchId = 406897,
            UserId = 8
        };

        var model7 = new RobotX3RawGroupModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 11:01:00 AM"),
            MatchGroup = 2,
            OWPMatchId = 406913,
            UserId = 8
        };

        var model8 = new RobotX3RawGroupModel
        {
            EventDateTime = DateTime.Parse("03/17/2020 1:00:00 PM"),
            MatchGroup = 3,
            OWPMatchId = 406934,
            UserId = 8
        };

        var model9 = new RobotX3RawGroupModel
        {
            EventDateTime = DateTime.Parse("03/18/2020 10:00:00 AM"),
            MatchGroup = 4,
            OWPMatchId = 406950,
            UserId = 8
        };

        var model10 = new RobotX3RawGroupModel
        {
            EventDateTime = DateTime.Parse("03/27/2020 2:00:00 PM"),
            MatchGroup = 5,
            OWPMatchId = 406987,
            UserId = 8
        };

        var assigmentList = new List<RobotX3RawModel> { model1, model2, model3, model4, model5 };
        var expected = new List<RobotX3RawGroupModel> { model6, model7, model8, model9, model10 };
        var actual = robotX3Service.GroupConecutiveMatch(assigmentList);
        for (var i = 0; i < actual.Count; i++)
        {
            Assert.True(actual[i].MatchGroup.Equals(expected[i].MatchGroup));
        }
    }

    [Fact]
    public void GroupConecutiveMatch_AssigmentEventDateTimeWithin30_TogetherGroup()
    {
        var assignmentList = new List<RobotX3RawModel>
            {
                 new RobotX3RawModel
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:30:00 PM"),
                      OWPMatchId = 347272,
                      UserId = 17013702
                 },
                 new RobotX3RawModel
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:45:00 PM"),
                      OWPMatchId = 347277,
                      UserId = 17013702
                 }
            };

        var expected = new List<RobotX3RawGroupModel>
            {
                 new RobotX3RawGroupModel
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:30:00 PM"),
                      MatchGroup = 1,
                      OWPMatchId = 347272,
                      UserId = 17013702
                 },
                 new RobotX3RawGroupModel
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:45:00 PM"),
                      MatchGroup = 1,
                      OWPMatchId = 347277,
                      UserId = 17013702
                 }
            };

        var actual = robotX3Service.GroupConecutiveMatch(assignmentList);
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
                 new RobotX3RawModel
                 {
                      EventDateTime = DateTime.Parse("03/16/2020 2:00:00 PM"),
                      OWPMatchId = 406897,
                      UserId = 8
                 },
                 new RobotX3RawModel
                 {
                      EventDateTime = DateTime.Parse("03/17/2020 11:01:00 AM"),
                      OWPMatchId = 406913,
                      UserId = 8
                 },
                 new RobotX3RawModel
                 {
                      EventDateTime = DateTime.Parse("03/17/2020 1:00:00 PM"),
                      OWPMatchId = 406934,
                      UserId = 8
                 },
                 new RobotX3RawModel
                 {
                      EventDateTime = DateTime.Parse("03/18/2020 10:00:00 AM"),
                      OWPMatchId = 406950,
                      UserId = 8
                 },
                  new RobotX3RawModel
                 {
                      EventDateTime = DateTime.Parse("03/27/2020 2:00:00 PM"),
                      OWPMatchId = 406987,
                      UserId = 8
                 },
                  new RobotX3RawModel
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:30:00 PM"),
                      OWPMatchId = 347272,
                      UserId = 17013702
                 },
                 new RobotX3RawModel
                 {
                      EventDateTime = DateTime.Parse("04/02/2020 1:45:00 PM"),
                      OWPMatchId = 347277,
                      UserId = 17013702
                 }
            };

        var expected = new List<RobotX3ByUser>
            {
                new RobotX3ByUser
                {
                    UserId = 8,
                    X2 = 0,
                    X3 = 0,
                    X3Plus = 0
                },
                new RobotX3ByUser
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