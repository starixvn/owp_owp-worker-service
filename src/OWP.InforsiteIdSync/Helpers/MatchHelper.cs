using Olympus.OWP.Constants;
using OWP.Core.Mics;
using OWP.InfositeIdSync.Models;

namespace OWP.InfositeIdSync.Helpers;

public static class MatchHelper
{
    public static List<SyncIsLiveModel> GetUnsynchronizedMatches(List<SyncIsLiveModel> owpMatches, List<MainDBMatch> db02Matches)
    {
        var mappedDB02Matches = MainDBMatchesMapping(owpMatches, db02Matches);

        var mainDB02Matches = mappedDB02Matches.Where(x => !x.MinorBettypeId.HasValue);
        var minorDB02Matches = mappedDB02Matches.Where(x => x.MinorBettypeId.HasValue);

        var mainOWPMatches = owpMatches.Where(x => !x.MinorBettypeId.HasValue);
        var minorOWPMatches = owpMatches.Where(x => x.MinorBettypeId.HasValue &&
            x.MinorBettypeId != MinorBettype.FifteenMinsNoOfCorners &&
            x.MinorBettypeId != MinorBettype.FifteenMinsTotalBooking);

        var unsynchronizedMainMatches = mainOWPMatches.Except(mainDB02Matches, new IsLiveComparer()).ToList();
        var unsynchronizedMinorMatches = GetUnsynchronizedMinorMatches(minorOWPMatches, minorDB02Matches);
        var unsynchronized15MinsMinorMatches = Get15MinsUnsynchronizedMinorMatches(unsynchronizedMinorMatches);

        return unsynchronizedMainMatches.Union(unsynchronizedMinorMatches).Union(unsynchronized15MinsMinorMatches).ToList();
    }

    private static IEnumerable<SyncIsLiveModel> Get15MinsUnsynchronizedMinorMatches(IEnumerable<SyncIsLiveModel> unsynchronizedMinorMatches)
    {
        return unsynchronizedMinorMatches.Where(m =>
                m.MinorBettypeId is MinorBettype.TotalBooking or MinorBettype.NoOfCorners)
            .Select(m => new SyncIsLiveModel
            {
                InfoSiteMatchId = m.InfoSiteMatchId,
                MinorBettypeId = (m.MinorBettypeId == MinorBettype.NoOfCorners
                    ? MinorBettype.FifteenMinsNoOfCorners
                    : MinorBettype.FifteenMinsTotalBooking),
                IsLive = m.IsLive
            });
    }

    internal static IEnumerable<(int, int?)> GetDistinctNonLiveMatches(IEnumerable<SyncIsLiveModel> liveMatches, IEnumerable<SyncIsLiveModel> nonLiveMatches)
        => nonLiveMatches
            .Except(liveMatches, new SyncIsLiveModel())
            .Select(x => (x.InfoSiteMatchId, GetRepresentativeSubType(x.MinorBettypeId)));

    internal static IEnumerable<(int, int?)> GetDistinctLiveMatches(IEnumerable<SyncIsLiveModel> liveMatches, IEnumerable<SyncIsLiveModel> nonLiveMatches)
        => liveMatches
            .Except(nonLiveMatches, new SyncIsLiveModel())
            .Select(x => (x.InfoSiteMatchId, GetRepresentativeSubType(x.MinorBettypeId)));

    private static IEnumerable<SyncIsLiveModel> GetUnsynchronizedMinorMatches(IEnumerable<SyncIsLiveModel> minorOWPMatches, IEnumerable<SyncIsLiveModel> minorDB02Matches)
    {
        return minorOWPMatches.Where(match => minorDB02Matches
            .Any(x => x.InfoSiteMatchId == match.InfoSiteMatchId
                      && x.MinorBettypeId == match.MinorBettypeId
                      && x.IsLive != match.IsLive));
    }

    private static List<SyncIsLiveModel> MainDBMatchesMapping(List<SyncIsLiveModel> owpMatches, List<MainDBMatch> db02Matches)
    {
        var mainMatches = db02Matches
            .Where(x => !x.ParentId.HasValue)
            .Select(x => new SyncIsLiveModel
            {
                InfoSiteMatchId = 1,
                IsLive = x.OfferLive,
                MinorBettypeId = null
            });

        var minorMatches = db02Matches
            .Where(x => x.ParentId.HasValue)
            .Select(x => MapMinorMatch(owpMatches, x));

        return mainMatches.Union(minorMatches).ToList();
    }

    private static SyncIsLiveModel MapMinorMatch(List<SyncIsLiveModel> owpMatches, MainDBMatch minorDB02Match)
    {
        var minorBettypeId = GetOWPMinorMatchIdFromSubtype(minorDB02Match.SubType);
        var mainInfositeMatchId = GetInfoSiteMatchIdForMainMatch(owpMatches, minorDB02Match.ParentId.Value);
        return new SyncIsLiveModel
        {
            MinorBettypeId = minorBettypeId,
            InfoSiteMatchId = mainInfositeMatchId,
            IsLive = minorDB02Match.OfferLive
        };
    }

    private static int GetInfoSiteMatchIdForMainMatch(List<SyncIsLiveModel> owpMatches, int matchId)
        => owpMatches.Find(m => m.AdminSiteMatchId == matchId)?.InfoSiteMatchId ?? 0;

    private static int? GetRepresentativeSubType(int? minorBettypeId)
        => !minorBettypeId.HasValue ? null : MinorBettype.GetRepresentativeSubtype(minorBettypeId.Value);

    public static int? GetOWPMinorMatchIdFromSubtype(int? subType)
    {
        if (!subType.HasValue)
        {
            return null;
        }

        var firstType = new Specific10MinsDetector();
        var secondType = new Specific15MinsDetector();
        var thirdType = new NextTypesDetector();
        var fourthType = new SingleTeamPenaltyShootOut();

        firstType.NextType = secondType;
        secondType.NextType = thirdType;
        thirdType.NextType = fourthType;

        return MinorBettype.GetMinorBettypeIdBySubtype(firstType.GetRepresentativeSubtype(subType.Value));
    }
}