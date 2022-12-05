using OWP.Core.Extensions;
using OWP.Infosite.Constants;
using OWP.Infosite.Models;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace OWP.Infosite.Helper;

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
    const string pattern = @"(?i)\d+\w{2}\s(Corner)";
    const string replaceText = "Next Corners";

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.NextCorner &&
            Regex.IsMatch(match.AwayName, pattern))
        {
            match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
            return MinorBettypeId.NextCorner;
        }

        return base.GetMinorType(match);
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.NextCorner)
        {
            match.HomeName = Regex.Replace(match.HomeName, pattern, replaceText);
            match.AwayName = Regex.Replace(match.AwayName, pattern, replaceText);
            return;
        }

        base.GetHomeAwayName(match);
    }
}

public class NextBookingRule : SpecialMatchGroupRule
{
    const string pattern = @"(?i)\d+\w{2}\s(Booking)";
    const string replaceText = "Next Booking";

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.NextBooking &&
           Regex.IsMatch(match.AwayName, pattern))
        {
            match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
            return MinorBettypeId.NextBooking;
        }

        return base.GetMinorType(match);
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.NextBooking)
        {
            match.HomeName = Regex.Replace(match.HomeName, pattern, replaceText);
            match.AwayName = Regex.Replace(match.AwayName, pattern, replaceText);
            return;
        }

        base.GetHomeAwayName(match);
    }
}

public class Specific15MinsRule : SpecialMatchGroupRule
{
    const string pattern = @"\d{2}:\d{2}-\d{2}:\d{2}";
    public static readonly ImmutableList<string> special15MinContain = new List<string>
       {"00:00-15:00",
        "15:01-30:00",
        "30:01-45:00",
        "45:01-60:00",
        "60:01-75:00",
        "75:01-90:00"}
    .ToImmutableList();
    const string replaceText = "(SPECIFIC 15 MINS)";

    private static bool CheckContainHomeAway(MappedChildMatch match, string typeSpecial)
        => match.AwayName.IsContainsIgnoreCase(typeSpecial) &&
           match.HomeName.IsContainsIgnoreCase(typeSpecial);

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.Specific15Min &&
            special15MinContain.Any(x => CheckContainHomeAway(match, x)))
        {
            match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
            return MinorBettypeId.Specific15Min;
        }

        return base.GetMinorType(match);
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.Specific15Min)
        {
            match.HomeName = Regex.Replace(match.HomeName, pattern, replaceText);
            match.AwayName = Regex.Replace(match.AwayName, pattern, replaceText);
            return;
        }

        base.GetHomeAwayName(match);
    }
}

public class GoalKickRule : SpecialMatchGroupRule
{
    const string pattern = @"(?i)\d+\w{2}\s(Goal Kick)$";
    const string replaceText = "Next Goal Kick";

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.NextGoalKick &&
           Regex.IsMatch(match.AwayName, pattern))
        {
            match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
            return MinorBettypeId.NextGoalKick;
        }

        return base.GetMinorType(match);
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.NextGoalKick)
        {
            match.HomeName = Regex.Replace(match.HomeName, pattern, replaceText);
            match.AwayName = Regex.Replace(match.AwayName, pattern, replaceText);
            return;
        }

        base.GetHomeAwayName(match);
    }
}

public class PenaltyToScoreRule : SpecialMatchGroupRule
{
    const string pattern = @"(?i)\d+\w{2}\s(Penalty To Score) - (\w+)";
    const string replaceAwayText = "1st Penalty To Score - Yes";

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.PenaltyToScore &&
           Regex.IsMatch(match.AwayName, pattern))
        {
            match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
            return MinorBettypeId.PenaltyToScore;
        }

        return base.GetMinorType(match);
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.PenaltyToScore)
        {
            var teamName = Regex.Replace(match.AwayName, pattern, replaceAwayText);
            string[] teamNames = teamName.Split(" -vs- ");

            match.HomeName = teamNames[0];
            match.AwayName = teamNames[1];
            return;
        }

        base.GetHomeAwayName(match);
    }
}

public class PenaltyAwardedRule : SpecialMatchGroupRule
{
    const string pattern = @"(?i)\d+\w{2}\s(Penalty Awarded) - (\w+)";
    const string replaceAwayText = "1st Penalty Awarded - Yes";

    public override int GetMinorType(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.PenaltyAwarded &&
           Regex.IsMatch(match.AwayName, pattern))
        {
            match.FirstMatch = ChildMatchHelper.IsFirstMatch(match.AwayName);
            return MinorBettypeId.PenaltyAwarded;
        }

        return base.GetMinorType(match);
    }

    public override void GetHomeAwayName(MappedChildMatch match)
    {
        if (match.MinorBettypeId == MinorBettypeId.PenaltyAwarded)
        {
            var teamName = Regex.Replace(match.AwayName, pattern, replaceAwayText);
            string[] teamNames = teamName.Split(" -vs- ");

            match.HomeName = teamNames[0];
            match.AwayName = teamNames[1];
            return;
        }
    }
}
