using OWP.SumReportData.TraderReport.Models;

namespace OWP.SumReportData.TraderReport.Services;

public interface RobotX5Service
{
    List<RobotX5ByUser> Aggregate(List<RobotX5RawModel> traderAssignments);

    int GetX5OfUser(List<RobotX5RawModel> assignmentsOfUser);

    bool IsX5(List<RobotX5RawGroupModel> assignments);

    List<RobotX5RawGroupModel> GroupByShiftAndMatchGroup(List<RobotX5RawModel> rawAssignments);

    int GetMinuteOfMatch(bool isFullTime, bool is35MinuteMatch);
}

public class RobotX5ServiceImp : RobotX5Service
{
    private const int MinMinutesForX5 = 400;

    private const int MinutesOfNormalFullTimeMatch = 90;

    private const int MinutesOfNormalHalfTimeMatch = 45;

    private const int MinutesOf35MinutesFullTimeMatch = 70;

    private const int MinutesOf35MinutesHalfTimeMatch = 35;

    private const int MaxMinutesForMultipleMatches = 30;

    private const int MaxMinutesBetweenTwoShifts = 5 * 60;

    public List<RobotX5ByUser> Aggregate(List<RobotX5RawModel> traderAssignments)
    {
        var assignmentsGroupByUsers = traderAssignments
            .GroupBy(x => x.UserId)
            .ToDictionary(x => x.Key, x => x.ToList());

        return assignmentsGroupByUsers.Select(m => new RobotX5ByUser
        {
            UserId = m.Key,
            X5 = GetX5OfUser(m.Value)
        })
            .ToList();
    }

    public int GetX5OfUser(List<RobotX5RawModel> assignmentsOfUser)
    {
        var assignmentsByShiftAndMatchGroup = GroupByShiftAndMatchGroup(assignmentsOfUser);
        var assignmentsByShift = assignmentsByShiftAndMatchGroup
            .GroupBy(a => a.Shift)
            .Select(i => i.ToList());

        return assignmentsByShift.Count(IsX5);
    }

    public bool IsX5(List<RobotX5RawGroupModel> assignments)
    {
        return assignments.GroupBy(i => i.MatchGroup)
            .Select(i => i.Max(g => g.Minutes))
            .Sum() >= MinMinutesForX5;
    }

    public List<RobotX5RawGroupModel> GroupByShiftAndMatchGroup(List<RobotX5RawModel> rawAssignments)
    {
        var assignments = rawAssignments
            .Select(i => new RobotX5RawGroupModel
            {
                EventDateTime = i.EventDateTime,
                OWPMatchId = i.OWPMatchId,
                UserId = i.UserId,
                Minutes = GetMinuteOfMatch(i.IsFullTime, i.Is35MinuteMatch),
                MatchGroup = 1,
                Shift = 1
            })
            .OrderBy(i => i.EventDateTime)
            .ToList();

        var group = 1;
        var shift = 1;

        for (var i = 1; i < assignments.Count; i++)
        {
            var timeSpanInMinute = (assignments[i].EventDateTime - assignments[i - 1].EventDateTime).TotalMinutes;
            group = timeSpanInMinute <= MaxMinutesForMultipleMatches ? group : group + 1;
            shift = timeSpanInMinute < MaxMinutesBetweenTwoShifts ? shift : shift + 1;

            assignments[i].MatchGroup = group;
            assignments[i].Shift = shift;
        }

        return assignments;
    }

    public int GetMinuteOfMatch(bool isFullTime, bool is35MinuteMatch)
    {
        var fullTimeMinutes = is35MinuteMatch ? MinutesOf35MinutesFullTimeMatch : MinutesOfNormalFullTimeMatch;
        var halfTimeMinutes = is35MinuteMatch ? MinutesOf35MinutesHalfTimeMatch : MinutesOfNormalHalfTimeMatch;

        return isFullTime ? fullTimeMinutes : halfTimeMinutes;
    }
}