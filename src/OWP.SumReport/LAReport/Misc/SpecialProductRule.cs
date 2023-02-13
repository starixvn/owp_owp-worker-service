namespace OWP.SumReport.LAReport.Misc;

using Constants;
using Enum;
using Models;

public class SpecialProductRule
{
    private SpecialProductRule nextRule;

    public void SetSuccessor(SpecialProductRule nextRule) => this.nextRule = nextRule;

    public virtual ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        return nextRule?.Execute(assignment, userAssignments) ?? ReportType.OTHER_SPECIAL_PRODUCTS;
    }
}

public class NotCornerRule : SpecialProductRule
{
    private readonly int[] cornerBettypeArray = { BettypeConst.NextCorner, BettypeConst.NoOfCorner, BettypeConst.NoOfCorner15Mins };
    private readonly int[] bookingBettypeArray = { BettypeConst.NextBooking, BettypeConst.TotalBooking, BettypeConst.TotalBooking15Mins };
    private readonly int[] goalKickBettypeArray = { BettypeConst.LastGoalKick, BettypeConst.NextGoalKick, BettypeConst.SecondHaft1stGoalKick };

    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        if (assignment.MinorBettypeId.HasValue && Array.IndexOf(cornerBettypeArray, assignment.MinorBettypeId.Value) < 0)
        {
            if (Array.IndexOf(bookingBettypeArray, assignment.MinorBettypeId.Value) >= 0)
            {
                return ReportType.TOTAL_BOOKING_PLUS;
            }

            if (Array.IndexOf(goalKickBettypeArray, assignment.MinorBettypeId.Value) >= 0)
            {
                return ReportType.GOAL_KICK;
            }

            return ReportType.OTHER_SPECIAL_PRODUCTS;
        }

        return base.Execute(assignment, userAssignments);
    }
}

public class IsCornerRule : SpecialProductRule
{
    private readonly int[] cornerBettypeArray = { BettypeConst.NextCorner, BettypeConst.NoOfCorner, BettypeConst.NoOfCorner15Mins };
    private static bool isMajorCornerLeague(string leagueName)
        => MajorLeague.majorLeagueCornerList.Any(x => x == leagueName);
    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        if (assignment.MinorBettypeId.HasValue && Array.IndexOf(cornerBettypeArray, assignment.MinorBettypeId.Value) >= 0)
        {
            var mainMatches = userAssignments.Where(x => !x.ParentMatchId.HasValue && x.OWPMatchId == assignment.ParentMatchId);

            if(isMajorCornerLeague(assignment.OWPLeagueName))
            {
                return mainMatches.Any() ? ReportType.FT_CORNER_5ML : ReportType.CORNER_PLUS;
            }

            return mainMatches.Any() ? ReportType.FT_CORNER : ReportType.CORNER_PLUS;
            
        }
        return base.Execute(assignment, userAssignments);
    }
}