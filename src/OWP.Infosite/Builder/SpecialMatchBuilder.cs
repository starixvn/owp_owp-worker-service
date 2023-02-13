using OWP.Infosite.Helper;
using OWP.Infosite.Models;

namespace OWP.Infosite.Builder;

public class SpecialMatchBuilder
{
    private readonly NextCornerRule firstRule;

    public SpecialMatchBuilder()
    {
        firstRule = new NextCornerRule();
        var secondRule = new NextBookingRule();
        var thirdRule = new Specific15MinsRule();
        var fourthRule = new GoalKickRule();
        var fifthRule = new PenaltyToScoreRule();
        var sixthRule = new PenaltyAwardedRule();

        firstRule.NextType = secondRule;
        secondRule.NextType = thirdRule;
        thirdRule.NextType = fourthRule;
        fourthRule.NextType = fifthRule;
        fifthRule.NextType = sixthRule;
    }

    public List<MappedChildMatch> ReduceSpecialMatch(IEnumerable<MappedChildMatch> childMatches)
    {
        var childMatchDict = childMatches
            .GroupBy(x => new { type = GetSpecialMatchType(x), x.ParentInfoSiteMatchId, x.EventDateTime })
            .ToDictionary(x => x.Key, x => x.ToList());
        var match = new List<MappedChildMatch>();

        foreach (var key in childMatchDict.Keys)
        {
            var firstMatch = childMatchDict[key].MaxBy(x => x.FirstMatch);
            if (key.type == 0)
            {
                match.AddRange(childMatchDict[key]);
            }
            else
            {
                match.Add(GetPresentationMatch(firstMatch));
            }
        }

        return match;
    }

    private int GetSpecialMatchType(MappedChildMatch childMatch)
        => firstRule.GetMinorType(childMatch);

    private MappedChildMatch GetPresentationMatch(MappedChildMatch childMatch)
    {
        firstRule.GetHomeAwayName(childMatch);
        childMatch.SubType = childMatch.RepresentativeSubtype;

        return childMatch;
    }
}
