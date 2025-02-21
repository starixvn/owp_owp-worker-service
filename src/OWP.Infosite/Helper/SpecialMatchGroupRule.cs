namespace OWP.Infosite.Helper;

using Constants;
using Extensions;
using Models;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

public class SpecialMatchGroupRule
{
    public SpecialMatchGroupRule NextType { get; set; }

    public virtual int GetMinorType(MappedChildMatch match)
        => NextType?.GetMinorType(match) ?? 0;

    public virtual void GetHomeAwayName(MappedChildMatch match)
        => NextType?.GetHomeAwayName(match);
}

public class NextCornerRule : SpecialMatchGroupRule
{
    private const string Pattern = @"(?i)\d+\w{2}\s(Corner)";
    private const string ReplaceText = "Next Corners";

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId != MinorBettypeId.NextCorner ||
            !Regex.IsMatch(match.AwayName, Pattern))
        {
            return base.GetMinorType(match);
        }

        match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
        return MinorBettypeId.NextCorner;
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.NextCorner)
        {
            match.HomeName = Regex.Replace(match.HomeName, Pattern, ReplaceText);
            match.AwayName = Regex.Replace(match.AwayName, Pattern, ReplaceText);
            return;
        }

        base.GetHomeAwayName(match);
    }
}

public class NextBookingRule : SpecialMatchGroupRule
{
    private const string Pattern = @"(?i)\d+\w{2}\s(Booking)";
    private const string ReplaceText = "Next Booking";

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId != MinorBettypeId.NextBooking ||
            !Regex.IsMatch(match.AwayName, Pattern))
        {
            return base.GetMinorType(match);
        }

        match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
        return MinorBettypeId.NextBooking;
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.NextBooking)
        {
            match.HomeName = Regex.Replace(match.HomeName, Pattern, ReplaceText);
            match.AwayName = Regex.Replace(match.AwayName, Pattern, ReplaceText);
            return;
        }

        base.GetHomeAwayName(match);
    }
}

public class Specific15MinsRule : SpecialMatchGroupRule
{
    private const string Pattern = @"\d{2}:\d{2}-\d{2}:\d{2}";
    public static readonly ImmutableList<string> Special15MinContain = new List<string>
       {"00:00-15:00",
        "15:01-30:00",
        "30:01-45:00",
        "45:01-60:00",
        "60:01-75:00",
        "75:01-90:00"}
    .ToImmutableList();

    private const string ReplaceText = "(SPECIFIC 15 MINS)";

    private static bool CheckContainHomeAway(MappedChildMatch match, string typeSpecial)
        => match.AwayName.IsContainsIgnoreCase(typeSpecial) &&
           match.HomeName.IsContainsIgnoreCase(typeSpecial);

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId != MinorBettypeId.Specific15Min ||
            !Special15MinContain.Exists(x => CheckContainHomeAway(match, x)))
        {
            return base.GetMinorType(match);
        }

        match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
        return MinorBettypeId.Specific15Min;
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.Specific15Min)
        {
            match.HomeName = Regex.Replace(match.HomeName, Pattern, ReplaceText);
            match.AwayName = Regex.Replace(match.AwayName, Pattern, ReplaceText);
            return;
        }

        base.GetHomeAwayName(match);
    }
}

public class GoalKickRule : SpecialMatchGroupRule
{
    private const string Pattern = @"(?i)\d+\w{2}\s(Goal Kick)$";
    private const string ReplaceText = "Next Goal Kick";

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId != MinorBettypeId.NextGoalKick ||
            !Regex.IsMatch(match.AwayName, Pattern))
        {
            return base.GetMinorType(match);
        }

        match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
        return MinorBettypeId.NextGoalKick;
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.NextGoalKick)
        {
            match.HomeName = Regex.Replace(match.HomeName, Pattern, ReplaceText);
            match.AwayName = Regex.Replace(match.AwayName, Pattern, ReplaceText);
            return;
        }

        base.GetHomeAwayName(match);
    }
}

public class PenaltyToScoreRule : SpecialMatchGroupRule
{
    private const string Pattern = @"(?i)\d+\w{2}\s(Penalty To Score) - (\w+)";
    private const string ReplaceAwayText = "1st Penalty To Score - Yes";

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId != MinorBettypeId.PenaltyToScore ||
            !Regex.IsMatch(match.AwayName, Pattern))
        {
            return base.GetMinorType(match);
        }

        match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
        return MinorBettypeId.PenaltyToScore;
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.PenaltyToScore)
        {
            var teamName = Regex.Replace(match.AwayName, Pattern, ReplaceAwayText);
            var teamNames = teamName.Split(" -vs- ");

            match.HomeName = teamNames[0];
            match.AwayName = teamNames[1];
            return;
        }

        base.GetHomeAwayName(match);
    }
}

public class PenaltyAwardedRule : SpecialMatchGroupRule
{
    private const string Pattern = @"(?i)\d+\w{2}\s(Penalty Awarded) - (\w+)";
    private const string ReplaceAwayText = "1st Penalty Awarded - Yes";

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId != MinorBettypeId.PenaltyAwarded ||
            !Regex.IsMatch(match.AwayName, Pattern))
        {
            return base.GetMinorType(match);
        }

        match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
        return MinorBettypeId.PenaltyAwarded;
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.PenaltyAwarded)
        {
            var teamName = Regex.Replace(match.AwayName, Pattern, ReplaceAwayText);
            var teamNames = teamName.Split(" -vs- ");

            match.HomeName = teamNames[0];
            match.AwayName = teamNames[1];
            return;
        }

        base.GetHomeAwayName(match);
    }
}
