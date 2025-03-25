using Olympus.OWP.Constants;
using OWP.Extensions;
using OWP.Infosite.Constants;
using OWP.Infosite.Models;
using System.Reflection;

namespace OWP.Infosite.Helper;

public static class InfoSiteSyncHelper
{
    public static int BuildActionId(string propertyName, string newValue, string oldValue)
    {
        return IsCancel(oldValue, newValue)
            ? BuildActionIdWithCancel(propertyName)
            : BuildActionIdWithUpdate(propertyName, newValue, oldValue);
    }

    private static int BuildActionIdWithUpdate(string propertyName, string newValue, string oldValue)
    {
        var propertyActionIds = new Dictionary<string, int>
        {
            { "EventDateTime", UpdateTypeConstant.UPDATE_KO_TIME},
            { "LeagueName", UpdateTypeConstant.UPDATE_LEAGUE },
            { "HomeName", UpdateTypeConstant.UPDATE_TEAM },
            { "AwayName", UpdateTypeConstant.UPDATE_TEAM },
            { "Tv", UpdateTypeConstant.UPDATE_TV },
            { "IPTV", UpdateTypeConstant.UPDATE_IPTV },
            { "OtherProvider", UpdateTypeConstant.UPDATE_OTHER_PROVIDER },
            { "Round", UpdateTypeConstant.UPDATE_ROUND },
            { "Remark", UpdateTypeConstant.UPDATE_REMARK },
            { "HomeRemark", UpdateTypeConstant.UPDATE_HOME_REMARK },
            { "AwayRemark", UpdateTypeConstant.UPDATE_AWAY_REMARK },
            { "Division", UpdateTypeConstant.UPDATE_DIVISION },
            { "RoundConfirm", UpdateTypeConstant.UPDATE_ROUND_CONFIRM },
            { "AdminSiteId", UpdateTypeConstant.UPDATE_ADMINSITEID },
            { "Website1", UpdateTypeConstant.UPDATE_WEB1 },
            { "Website2", UpdateTypeConstant.UPDATE_WEB2 },
            { "Website3", UpdateTypeConstant.UPDATE_WEB3 },
            { "Website4", UpdateTypeConstant.UPDATE_WEB4 },
            { "WatchBet", UpdateTypeConstant.UPDATE_LINK },
            { "Channel", UpdateTypeConstant.UPDATE_CHANNEL },
            { "Agent", UpdateTypeConstant.UPDATE_AGENT },
            { "Venue", UpdateTypeConstant.UPDATE_VENUE },
            { "Etpk", UpdateTypeConstant.UPDATE_ETPK },
            { "IsNeutral", UpdateTypeConstant.UPDATE_NEUTRAL },
            { "Remark2", UpdateTypeConstant.UPDATE_REMARK2},
            { "Provider", UpdateTypeConstant.UPDATE_PROVIDER},
            { "TotalRound", UpdateTypeConstant.UPDATE_TOTAL_ROUND},
        };
        var actionId = propertyActionIds.GetValue(propertyName, 1);
        
        return actionId;
    }

   

   

    private static int BuildActionIdWithCancel(string propertyName)
    {
        var propertyActionIds = new Dictionary<string, int>
        {
            { "Tv", UpdateTypeConstant.CANCEL_TV },
            { "IPTV", UpdateTypeConstant.CANCEL_IPTV},
            { "OtherProvider", UpdateTypeConstant.CANCEL_OTHER_PROVIDER },
            { "Round", UpdateTypeConstant.CANCEL_ROUND },
            { "TeamRound", UpdateTypeConstant.CANCEL_ROUND_HAT },
            { "Remark", UpdateTypeConstant.CANCEL_REMARK },
            { "HomeRemark", UpdateTypeConstant.CANCEL_HOME_REMARK },
            { "AwayRemark", UpdateTypeConstant.CANCEL_AWAY_REMARK },
            { "Division", UpdateTypeConstant.CANCEL_DIVISION },
            { "RoundConfirm", UpdateTypeConstant.CANCEL_ROUND_CONFIRM },
            { "AdminSiteId", UpdateTypeConstant.CANCEL_ADMINSITEID },
            { "Website1", UpdateTypeConstant.CANCEL_WEB1 },
            { "Website2", UpdateTypeConstant.CANCEL_WEB2 },
            { "Website3", UpdateTypeConstant.CANCEL_WEB3 },
            { "Website4", UpdateTypeConstant.CANCEL_WEB4 },
            { "WatchBet", UpdateTypeConstant.CANCEL_LINK },
            { "Channel", UpdateTypeConstant.CANCEL_CHANNEL },
            { "Agent", UpdateTypeConstant.CANCEL_AGENT },
            { "Venue", UpdateTypeConstant.CANCEL_VENUE },
            { "Etpk", UpdateTypeConstant.CANCEL_ETPK },
            { "IsNeutral", UpdateTypeConstant.CANCEL_NEUTRAL },
            { "Remark2", UpdateTypeConstant.CANCEL_REMARK2},
            { "Provider", UpdateTypeConstant.CANCEL_PROVIDER},
            { "TotalRound", UpdateTypeConstant.CANCEL_TOTAL_ROUND},
        };

        return propertyActionIds.GetValue(propertyName, 1);
    }

    private static bool IsCancel(string oldValue, string newValue)
        => oldValue != string.Empty && newValue == string.Empty;

    public static DateTime DateTimeNowGmt8()
        => DateTime.Now.AddHours(12);

    public static InfoSiteDb CloneInfoSiteMatch(Event source)
        => new()
        {
            InfoSiteMatchId = source.InfoSiteMatchId,
            LeagueName = source.LeagueName,
            AwayName = source.AwayName,
            HomeName = source.HomeName,
            EventDateTime = source.EventDateTime,
            IPTV = source.Iptv,
            Tv = source.LocalTv,
            Provider = source.Provider,
            OtherProvider = source.OtherProvider,
            AwayRemark = source.AwayRemark,
            Division = source.Division,
            HomeRemark = source.HomeRemark,
            Remark = source.Remark,
            Round = source.Round,
            TotalRound = source.TotalRound,
            RoundConfirm = source.RoundConfirm,
            AdminSiteId = source.AdminSiteMatchId,
            Website1 = source.Website1,
            Website2 = source.Website2,
            Website3 = source.Website3,
            Website4 = source.Website4,
            WatchBet = source.WatchBet,
            Channel = source.LiveTv,
            Agent = source.Agent,
            Venue = source.Venue,
            Etpk = source.Etpk,
            IsNeutral = source.IsNeutral,
            Remark2 = source.Remark2
        };

    public static IEnumerable<LoggedChangeModel> BuildLoggedChanges(
        IEnumerable<PropertyInfo> properties,
        InfoSiteDb infoSiteOld,
        InfoSiteDb infoSiteNew,
        string fromService = "Today")
    {
        return properties.Select(property =>
            {
                var newValue = property.GetValue(infoSiteNew)?.ToString() ?? string.Empty;
                var oldValue = property.GetValue(infoSiteOld)?.ToString() ?? string.Empty;

                if (string.Equals(newValue, oldValue))
                {
                    return null;
                }

                var updateTypeId = BuildActionId(property.Name, newValue, oldValue);

                return new LoggedChangeModel
                {
                    InfoSiteMatchId = infoSiteNew.InfoSiteMatchId,
                    UpdateTypeId = updateTypeId,
                    Action = UpdateTypeConstant.UpdateType.GetValueOrDefault(updateTypeId),
                    NewValue = newValue,
                    OldValue = oldValue,
                    HomeUpdated = string.Equals(property.Name, "HomeName", StringComparison.OrdinalIgnoreCase),
                    AwayUpdated = string.Equals(property.Name, "AwayName", StringComparison.OrdinalIgnoreCase),
                    UpDateTime = DateTimeNowGmt8(),
                    FromService = fromService
                };
            })
            .Where(log => log != null);
    }

    public static string GetStatusFromRemark2(this string input)
    {
        var statuses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { MatchStatusInRemark2.Postponed, MatchStatusInRemark2.Postponed },
            { MatchStatusInRemark2.Cancel, "Cancelled" }
        };

        foreach (var status in statuses)
        {
            if (input.Contains(status.Key, StringComparison.OrdinalIgnoreCase))
            {
                return status.Value;
            }
        }

        return string.Empty;
    }

    public static string GetStatusFromMatchStatus(this byte? input)
    {
        var statuses = new Dictionary<byte, string>()
        {
            { InfoSiteMatchStatus.Postponed, "Postponed" },
            { InfoSiteMatchStatus.Cancelled, "Cancelled" },
            { InfoSiteMatchStatus.Suspended, "Suspended" }
        };

        return input.HasValue ? statuses.GetValueOrDefault(input.Value, "") : "";
    }

    public static short GetUpdateTypeIdFromStatus(this string input)
    {
        var statuses = new Dictionary<string, short>(StringComparer.OrdinalIgnoreCase)
        {
            { "Postponed", UpdateTypeConstant.MATCH_POSTPONED },
            { "Cancelled", UpdateTypeConstant.MATCH_CANCELLED },
            { "Suspended", UpdateTypeConstant.MATCH_SUSPENDED },
            { "", UpdateTypeConstant.RENEW_MATCH }
        };

        return statuses.GetValueOrDefault<string, short>(input, 0);
    }
}