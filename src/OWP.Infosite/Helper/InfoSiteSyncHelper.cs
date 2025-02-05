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
            { "EventDateTime", 2 },
            { "LeagueName", 3 },
            { "HomeName", 4 },
            { "AwayName", 4 },
            { "Tv", 5 },
            { "IPTV", 7 },
            { "OtherProvider", 9 },
            { "Round", 12 },
            { "Remark", 16 },
            { "HomeRemark", 18 },
            { "AwayRemark", 20 },
            { "Division", 22 },
            { "RoundConfirm", 28 },
            { "AdminSiteId", 30 },
            { "Website1", 32 },
            { "Website2", 34 },
            { "Website3", 36 },
            { "Website4", 38 },
            { "WatchBet", 40 },
            { "Channel", 42 },
            { "Agent", 44 },
            { "Venue", 46 },
            { "Etpk", 48 },
            { "IsNeutral", 50 }
        };
        var actionId = propertyActionIds.GetValue(propertyName, 1);
        if (actionId == 2 && IsUpdateKODate(newValue, oldValue))
        {
            actionId = 24;
        }
        return actionId;
    }

    private static bool IsUpdateKODate(string newValue, string oldValue)
    {
        var newKODateTime = DateTime.Parse(newValue);
        var oldKODateTime = DateTime.Parse(oldValue);

        var milestoneDate = IsKOTimeAfter12pm(oldKODateTime) ? oldKODateTime.Date : oldKODateTime.Date.AddDays(-1);

        var from = milestoneDate.AddHours(12);
        var to = milestoneDate.AddDays(1).AddHours(12).AddSeconds(-1);

        return newKODateTime < from || newKODateTime > to;
    }

    private static bool IsKOTimeAfter12pm(DateTime koTime)
        => koTime.TimeOfDay >= new TimeSpan(12, 0, 0);

    private static int BuildActionIdWithCancel(string propertyName)
    {
        var propertyActionIds = new Dictionary<string, int>
        {
            { "Tv", 6 },
            { "IPTV", 8 },
            { "OtherProvider", 10 },
            { "Round", 13 },
            { "TeamRound", 15 },
            { "Remark", 17 },
            { "HomeRemark", 19 },
            { "AwayRemark", 21 },
            { "Division", 23 },
            { "RoundConfirm", 29 },
            { "AdminSiteId", 31 },
            { "Website1", 33 },
            { "Website2", 35 },
            { "Website3", 37 },
            { "Website4", 39 },
            { "WatchBet", 41 },
            { "Channel", 43 },
            { "Agent", 45 },
            { "Venue", 47 },
            { "Etpk", 49 },
            { "IsNeutral", 51 }
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
            OtherProvider = source.OtherProvider,
            AwayRemark = source.AwayRemark,
            Division = source.Division,
            HomeRemark = source.HomeRemark,
            Remark = source.Remark,
            Round = source.Round,
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
                    Action = Constant.UpdateType.GetValueOrDefault(updateTypeId),
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
}