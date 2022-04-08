using System.Linq;
using OWP.Core.Extensions;
using OWP.SumReportData.LAReport.Constants;
using OWP.SumReportData.LAReport.Enum;
using OWP.SumReportData.LAReport.Models;

namespace OWP.SumReportData.LAReport.Misc;

public class MainMatchRule
{
    private MainMatchRule nextRule;

    public void SetSuccessor(MainMatchRule nextRule) => this.nextRule = nextRule;

    public virtual ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        return nextRule?.Execute(assignment, userAssignments) ?? ReportType.OTHERS;
    }
}

public class FTCornerRule : MainMatchRule
{
    private readonly int[] cornerBettypeArray = { BettypeConst.NextCorner, BettypeConst.NoOfCorner, BettypeConst.NoOfCorner15Mins };

    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        return isContainCorner(assignment, userAssignments) ? ReportType.FT_CORNER : base.Execute(assignment, userAssignments);
    }

    private bool isContainCorner(Assignment assignment, IList<Assignment> assignments)
    {
        return assignments.Any(x => x.MinorBettypeId.HasValue && cornerBettypeArray.Contains(x.MinorBettypeId.Value) && x.ParentMatchId == assignment.OWPMatchId);
    }
}

public class IsFantasyRule : MainMatchRule
{
    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        return IsFantasy(assignment.OWPLeagueName) ? ReportType.FANTASY : base.Execute(assignment, userAssignments);
    }

    private static bool IsFantasy(string leagueName)
        => leagueName.IsContainsIgnoreCase("FANTASY MATCH");
}

public class ESoccerRule : MainMatchRule
{
    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        if (isESoccer(assignment.OWPLeagueName))
        {
            if (assignment.OWPLeagueName.IsContainsIgnoreCase("- 5 Mins Play"))
            {
                return ReportType.ESOCCER_5MINS;
            }

            if (assignment.OWPLeagueName.IsContainsIgnoreCase("- 6 Mins Play"))
            {
                return ReportType.ESOCCER_6MINS;
            }

            if (assignment.OWPLeagueName.IsContainsIgnoreCase("- 15 Mins Play"))
            {
                return ReportType.ESOCCER_15MINS;
            }

            if (assignment.OWPLeagueName.IsContainsIgnoreCase("- 16 Mins Play"))
            {
                return ReportType.ESOCCER_16MINS;
            }

            if (assignment.OWPLeagueName.IsContainsIgnoreCase("- 20 Mins Play"))
            {
                return ReportType.ESOCCER_20MINS;
            }
        }

        return base.Execute(assignment, userAssignments);
    }

    private static bool isESoccer(string leagueName)
        => leagueName.IsContainsIgnoreCase("SABA ");
}
public class FiveMajorLeagueRule : MainMatchRule
{
    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        if (isMajorLeague(assignment.OWPLeagueName))
        {
            return ReportType.MAJOR_LEAGUE;
        }

        return base.Execute(assignment, userAssignments);
    }

    private static bool isMajorLeague(string leagueName)
        => MajorLeague.majorLeagueList.Any( x => x == leagueName);
}