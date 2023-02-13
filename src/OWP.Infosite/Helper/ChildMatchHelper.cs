using Olympus.OWP.Constants;
using OWP.Core.Mics;

namespace OWP.Infosite.Helper;

public static class ChildMatchHelper
{
    public static (int, int) GetBetTypeIdAndRepresentativeSubtype(int? subType)
    {
        var firstType = new Specific10MinsDetector();
        var secondType = new Specific15MinsDetector();
        var thirdType = new NextTypesDetector();
        var fourthType = new SingleTeamPenaltyShootOut();
        var fifthType = new WinnerAndNextRound();

        firstType.NextType = secondType;
        secondType.NextType = thirdType;
        thirdType.NextType = fourthType;
        fourthType.NextType = fifthType;
        var representativeSubtype = firstType.GetRepresentativeSubtype(subType.Value);
        var minorBetTypeId = MinorBettype.GetMinorBettypeIdBySubtype(representativeSubtype);

        return (representativeSubtype, minorBetTypeId);
    }

    public static bool IsFirstMatch(string awayName)
        => awayName.Contains(" 1st ", StringComparison.OrdinalIgnoreCase);
}