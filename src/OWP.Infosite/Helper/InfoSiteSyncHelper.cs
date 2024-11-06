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
            { "Division", 22 }
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
        DateTime newKODateTime = DateTime.Parse(newValue);
        DateTime oldKODateTime = DateTime.Parse(oldValue);

        DateTime milestoneDate = IsKOTimeAfter12pm(oldKODateTime) ? oldKODateTime.Date : oldKODateTime.Date.AddDays(-1);

        DateTime from = milestoneDate.AddHours(12);
        DateTime to = milestoneDate.AddDays(1).AddHours(12).AddSeconds(-1);

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
            { "Division", 23 }
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
            Round = source.Round
        };

    public static IEnumerable<LoggedChangeModel> BuildLoggedChanges(IEnumerable<PropertyInfo> properties, InfoSiteDb infoSiteOld, InfoSiteDb infoSiteNew, string fromService = "Today")
    {
        var newValue = string.Empty;
        var oldValue = string.Empty;

        return properties.Where(property =>
            {
                newValue = property.GetValue(infoSiteNew)?.ToString() ?? string.Empty;
                oldValue = property.GetValue(infoSiteOld)?.ToString() ?? string.Empty;

                return !string.Equals(newValue, oldValue);
            })
            .Select(property =>
            {
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
            });
    }
}