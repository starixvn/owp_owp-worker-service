using OWP.Adminsite.Constants;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Builder;

public static class TraderAssignmentBuilder
{
    public static void BuildTraderAssignment(MatchInfo matchInfo, MatchInfoApiModel matchInfoApi, IEnumerable<Assignment> assignments)
    {
        var traders = assignments.Where(x => x.OWPMatchId == matchInfo.OWPMatchId).ToList();

        GetSuspenderTrader(matchInfoApi, traders);

        matchInfoApi.Traders = traders.Where(x => x.BettypeId != (int)MajorBetType.LA)
            .Select(x => new AssignmentApiModel
            {
                BettypeId = x.BettypeId,
                UserId = x.UserId,
                Username = x.Username
            }).ToList();
    }

    public static void BuildTraderAssignmentSpecial(MatchInfo matchInfo, MatchInfoApiModel matchInfoApi, IEnumerable<Assignment> assignments)
    {
        if (SpecialBettype.BettypeIdSpecificXMins.Any(id => id == matchInfo.MinorBettypeId))
        {
            var traders = assignments.Where(x => x.OWPMatchId == matchInfo.OWPMatchId).ToList();
            GetSuspenderTrader(matchInfoApi, traders);

            var traderHandicap = traders.FirstOrDefault(x => x.OWPMatchId == matchInfo.OWPMatchId);
            if (traderHandicap != null)
            {
                matchInfoApi.Traders =
                [
                    new AssignmentApiModel { BettypeId = (int)MajorBetType.Handicap, UserId = traderHandicap.UserId, Username = traderHandicap.Username },
                    new AssignmentApiModel { BettypeId = (int)MajorBetType.OverUnder, UserId = traderHandicap.UserId, Username = traderHandicap.Username }
                ];
            }
        }
        else
        {
            BuildTraderAssignment(matchInfo, matchInfoApi, assignments);
        }
    }

    private static void GetSuspenderTrader(MatchInfoApiModel matchInfoApi, IEnumerable<Assignment> assignments)
    {
        var suspenderTrader = assignments.FirstOrDefault(x => x.BettypeId == (int)MajorBetType.LA);

        if (suspenderTrader != null)
        {
            matchInfoApi.SuspenderUserId = suspenderTrader.UserId;
            matchInfoApi.SuspenderUsername = suspenderTrader.Username;
        }
    }
}