using OWP.SumReportData.TraderReport.Models;

namespace OWP.SumReportData.TraderReport.Services;

public interface RobotX3Service
{
    List<RobotX3ByUser> Aggregate(List<RobotX3RawModel> assignments);

    RobotX3ByUser Transform(int userId, List<RobotX3RawModel> assignments);

    List<RobotX3RawGroupModel> GroupConecutiveMatch(List<RobotX3RawModel> assignmentsOfUser);
}

public class RobotX3ServiceImp : RobotX3Service
{
    private const int X2 = 2;

    private const int X3 = 3;

    private const int MaxMinutesForMultipleMatches = 30;

    public List<RobotX3ByUser> Aggregate(List<RobotX3RawModel> assignments)
    {
        var traderAssignmentByUsers = assignments
            .GroupBy(x => x.UserId)
            .Select(x => new
            {
                UserId = x.Key,
                Assignments = x.ToList()
            })
            .ToList();

        return traderAssignmentByUsers.Select(m => Transform(m.UserId, m.Assignments))
            .ToList();
    }

    public RobotX3ByUser Transform(int userId, List<RobotX3RawModel> assignments)
    {
        var consecutiveAssignments = GroupConecutiveMatch(assignments);
        var matchGroupAsignments = consecutiveAssignments.GroupBy(i => i.MatchGroup)
            .Select(i => new { MatchGroup = i, MatchCount = i.Count() })
            .ToArray();

        return new RobotX3ByUser
        {
            UserId = userId,
            X2 = matchGroupAsignments.Count(x => x.MatchCount == X2),
            X3 = matchGroupAsignments.Count(x => x.MatchCount == X3),
            X3Plus = matchGroupAsignments.Count(x => x.MatchCount > X3)
        };
    }

    public List<RobotX3RawGroupModel> GroupConecutiveMatch(List<RobotX3RawModel> assignmentsOfUser)
    {
        var sortedList = assignmentsOfUser
            .Select(i => new RobotX3RawGroupModel
            {
                EventDateTime = i.EventDateTime,
                MatchGroup = 1,
                OWPMatchId = i.OWPMatchId,
                UserId = i.UserId
            })
            .OrderBy(i => i.EventDateTime)
            .ToList();

        var group = 1;

        for (var i = 1; i < sortedList.Count; i++)
        {
            var timeSpanInMinute = (sortedList[i].EventDateTime - sortedList[i - 1].EventDateTime).TotalMinutes;
            group = timeSpanInMinute <= MaxMinutesForMultipleMatches ? group : group + 1;
            sortedList[i].MatchGroup = group;
        }

        return sortedList;
    }
}