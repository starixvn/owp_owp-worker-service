namespace OWP.SumReport.LAReport.Misc;

using Constants;
using Enum;
using Extensions;
using Models;

public class MainMatchRule
{
    private MainMatchRule nextRule;

    public void SetSuccessor(MainMatchRule rule) => nextRule = rule;

    public virtual ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        return nextRule?.Execute(assignment, userAssignments) ?? ReportType.Others;
    }
}

public class FtCornerRule : MainMatchRule
{
    private readonly int[] cornerBettypeArray = { BettypeConst.NextCorner, BettypeConst.NoOfCorner, BettypeConst.NoOfCorner15Mins };

    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        return IsContainCorner(assignment, userAssignments)
            ? ReportType.FtCorner
            : base.Execute(assignment, userAssignments);
    }

    private bool IsContainCorner(Assignment assignment, IEnumerable<Assignment> assignments)
    {
        return assignments.Any(x =>
            x.MinorBettypeId.HasValue && cornerBettypeArray.Contains(x.MinorBettypeId.Value) &&
            x.ParentMatchId == assignment.OwpMatchId);
    }
}

public class FtCorner5MlRule : MainMatchRule
{
    private readonly int[] cornerBettypeArray = { BettypeConst.NextCorner, BettypeConst.NoOfCorner, BettypeConst.NoOfCorner15Mins };

    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        return IsContainCorner5Ml(assignment, userAssignments)
            ? ReportType.FtCorner5Ml
            : base.Execute(assignment, userAssignments);
    }

    private static bool IsMajorCornerLeague(string leagueName)
    {
        return MajorLeague.MajorLeagueCornerList.Exists(x => x == leagueName);
    }

    private bool IsContainCorner5Ml(Assignment assignment, IEnumerable<Assignment> assignments)
    {
        return assignments.Any(x =>
            x.MinorBettypeId.HasValue && cornerBettypeArray.Contains(x.MinorBettypeId.Value) &&
            x.ParentMatchId == assignment.OwpMatchId && IsMajorCornerLeague(x.OwpLeagueName));
    }
}

public class IsFantasyRule : MainMatchRule
{
    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        return IsFantasy(assignment.OwpLeagueName) ? ReportType.Fantasy : base.Execute(assignment, userAssignments);
    }

    private static bool IsFantasy(string leagueName)
        => leagueName.IsContainsIgnoreCase("FANTASY MATCH");
}

public class ESoccerRule : MainMatchRule
{
    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        if (!IsESoccer(assignment.OwpLeagueName))
        {
            return assignment.OwpLeagueName.IsContainsIgnoreCase("- 20 Mins Play")
                ? ReportType.Esoccer20Mins
                : base.Execute(assignment, userAssignments);
        }

        if (assignment.OwpLeagueName.IsContainsIgnoreCase("- 5 Mins Play"))
        {
            return ReportType.Esoccer5Mins;
        }

        if (assignment.OwpLeagueName.IsContainsIgnoreCase("- 6 Mins Play")
        || (assignment.OwpLeagueName.IsContainsIgnoreCase("SABA")
        && assignment.OwpLeagueName.IsContainsIgnoreCase("- PENALTY SHOOTOUTS")))
        {
            return ReportType.Esoccer6Mins;
        }

        if (assignment.OwpLeagueName.IsContainsIgnoreCase("- 15 Mins Play"))
        {
            return ReportType.Esoccer15Mins;
        }

        if (assignment.OwpLeagueName.IsContainsIgnoreCase("- 16 Mins Play"))
        {
            return ReportType.Esoccer16Mins;
        }

        return assignment.OwpLeagueName.IsContainsIgnoreCase("- 20 Mins Play")
            ? ReportType.Esoccer20Mins
            : base.Execute(assignment, userAssignments);
    }

    private static bool IsESoccer(string leagueName)
        => leagueName.IsContainsIgnoreCase("SABA ");
}

public class FiveMajorLeagueRule : MainMatchRule
{
    public override ReportType Execute(Assignment assignment, IList<Assignment> userAssignments)
    {
        return IsMajorLeague(assignment.OwpLeagueName)
            ? ReportType.MajorLeague
            : base.Execute(assignment, userAssignments);
    }

    private static bool IsMajorLeague(string leagueName)
        => MajorLeague.MajorLeagueList.Exists(x => x == leagueName);
}