using OWP.Adminsite.Constants;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;
using System.Collections.ObjectModel;

namespace OWP.Adminsite.Builder;

public static class NumberOfLinesBuilder
{
    private static readonly ReadOnlyDictionary<NoOfHdp, NumberOfLines> numberOfLinesDict = new(
        new Dictionary<NoOfHdp, NumberOfLines>
        {
            { NoOfHdp.TwoPlusFourLines, new NumberOfLines { CreditFt = 2, CreditFirstHalf = 2, LicenseeFt = 4, LicenseeFirstHalf = 3 } },
            { NoOfHdp.ThreePlusThreeLines, new NumberOfLines { CreditFt = 3, CreditFirstHalf = 2, LicenseeFt = 3, LicenseeFirstHalf = 2 } },
            { NoOfHdp.ThreePlusFourLines, new NumberOfLines { CreditFt = 3, CreditFirstHalf = 2, LicenseeFt = 4, LicenseeFirstHalf = 3 } },
            { NoOfHdp.FourPlusFourLines, new NumberOfLines { CreditFt = 4, CreditFirstHalf = 3, LicenseeFt = 4, LicenseeFirstHalf = 3 } },
            { NoOfHdp.OneLine, new NumberOfLines { CreditFt = 1, CreditFirstHalf = 1, LicenseeFt = 0, LicenseeFirstHalf = 0 } },
            { NoOfHdp.TwoLines, new NumberOfLines { CreditFt = 2, CreditFirstHalf = 2, LicenseeFt = 0, LicenseeFirstHalf = 0 } },
            { NoOfHdp.Empty, new NumberOfLines { CreditFt = 2, CreditFirstHalf = 2, LicenseeFt = 3, LicenseeFirstHalf = 2 } }
        }
    );

    private static readonly ReadOnlyDictionary<NoOfHdp, NumberOfLines> numberOfLinesSpecialProductDict = new(
        new Dictionary<NoOfHdp, NumberOfLines>
        {
            { NoOfHdp.TwoPlusFourLines, new NumberOfLines { CreditFt = 2 , CreditFirstHalf = 2, LicenseeFt = 4, LicenseeFirstHalf = 3 } },
            { NoOfHdp.ThreePlusThreeLines, new NumberOfLines { CreditFt = 3 , CreditFirstHalf = 2, LicenseeFt = 3, LicenseeFirstHalf = 2 } },
            { NoOfHdp.ThreePlusFourLines, new NumberOfLines { CreditFt = 3 , CreditFirstHalf = 2, LicenseeFt = 4, LicenseeFirstHalf = 3 } },
            { NoOfHdp.FourPlusFourLines, new NumberOfLines { CreditFt = 4 , CreditFirstHalf = 3, LicenseeFt = 4, LicenseeFirstHalf = 3 } },
            { NoOfHdp.OneLine, new NumberOfLines { CreditFt = 1 , CreditFirstHalf = 1, LicenseeFt = 0, LicenseeFirstHalf = 0 } },
            { NoOfHdp.TwoLines, new NumberOfLines { CreditFt = 2 , CreditFirstHalf = 2, LicenseeFt = 0, LicenseeFirstHalf = 0 } },
            { NoOfHdp.Empty, new NumberOfLines { CreditFt = null , CreditFirstHalf = null, LicenseeFt = null, LicenseeFirstHalf = null } }
        }
    );

    public static void BuildNumberOfLines(MatchInfo matchInfo, MatchInfoApiModel matchInfoApi)
    {
        var numberOfLines = GetNumberOfLines(matchInfo.NoOfHDPId);
        matchInfoApi.CreditFtLines = numberOfLines.CreditFt;
        matchInfoApi.CreditFirstHalfLines = numberOfLines.CreditFirstHalf;
        matchInfoApi.LicenseeFtLines = numberOfLines.LicenseeFt;
        matchInfoApi.LicenseeFirstHalfLines = numberOfLines.LicenseeFirstHalf;
    }

    public static void BuildNumberOfLinesSpecialProduct(MatchInfo matchInfo, MatchInfoApiModel matchInfoApi)
    {
        if (!Array.Exists(SpecialBettype.BettypeIdSpecificXMins, id => id == matchInfo.MinorBettypeId))
        {
            var numberOfLines = GetNumberOfLinesSpecialProduct(matchInfo.NoOfHDPId);
            matchInfoApi.CreditFtLines = numberOfLines.CreditFt;
            matchInfoApi.CreditFirstHalfLines = numberOfLines.CreditFirstHalf;
            matchInfoApi.LicenseeFtLines = numberOfLines.LicenseeFt;
            matchInfoApi.LicenseeFirstHalfLines = numberOfLines.LicenseeFirstHalf;
        }
    }

    private static NumberOfLines GetNumberOfLines(int noOfHdpId)
        => !numberOfLinesDict.TryGetValue((NoOfHdp)noOfHdpId, out var result) ? new NumberOfLines() : result;

    private static NumberOfLines GetNumberOfLinesSpecialProduct(int noOfHdpId)
        => !numberOfLinesSpecialProductDict.TryGetValue((NoOfHdp)noOfHdpId, out var result) ? new NumberOfLines() : result;
}