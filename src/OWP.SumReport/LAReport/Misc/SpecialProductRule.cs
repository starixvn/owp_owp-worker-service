namespace OWP.SumReport.LAReport.Misc;

using Constants;
using Enum;
using Models;

public class SpecialProductRule
{
    private SpecialProductRule nextRule;

    public void SetSuccessor(SpecialProductRule rule) => nextRule = rule;

    public virtual ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        return nextRule?.Execute(assignment, userAssignments) ?? ReportType.OtherSpecialProducts;
    }
}

public class NotCornerRule : SpecialProductRule
{
    private readonly int[] cornerBettypeArray = { BettypeConst.NextCorner, BettypeConst.NoOfCorner, BettypeConst.NoOfCorner15Mins };
    private readonly int[] bookingBettypeArray = { BettypeConst.NextBooking, BettypeConst.TotalBooking, BettypeConst.TotalBooking15Mins };
    private readonly int[] goalKickBettypeArray = { BettypeConst.LastGoalKick, BettypeConst.NextGoalKick, BettypeConst.SecondHaft1StGoalKick };

    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        if (!assignment.MinorBettypeId.HasValue ||
            Array.IndexOf(cornerBettypeArray, assignment.MinorBettypeId.Value) >= 0)
        {
            return base.Execute(assignment, userAssignments);
        }

        if (Array.IndexOf(bookingBettypeArray, assignment.MinorBettypeId.Value) >= 0)
        {
            return ReportType.TotalBookingPlus;
        }

        return Array.IndexOf(goalKickBettypeArray, assignment.MinorBettypeId.Value) >= 0 
            ? ReportType.GoalKick 
            : ReportType.OtherSpecialProducts;
    }
}

public class IsCornerRule : SpecialProductRule
{
    private readonly int[] cornerBettypeArray = { BettypeConst.NextCorner, BettypeConst.NoOfCorner, BettypeConst.NoOfCorner15Mins };
    private static bool IsMajorCornerLeague(string leagueName)
        => MajorLeague.MajorLeagueCornerList.Any(x => x == leagueName);
    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        if (!assignment.MinorBettypeId.HasValue ||
            Array.IndexOf(cornerBettypeArray, assignment.MinorBettypeId.Value) < 0)
        {
            return base.Execute(assignment, userAssignments);
        }
        
        var mainMatches = userAssignments.Where(x => !x.ParentMatchId.HasValue && x.OwpMatchId == assignment.ParentMatchId);

        if(IsMajorCornerLeague(assignment.OwpLeagueName))
        {
            return mainMatches.Any() ? ReportType.FtCorner5Ml : ReportType.CornerPlus;
        }

        return mainMatches.Any() ? ReportType.FtCorner : ReportType.CornerPlus;
    }
}