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
            : BuildActionIdWithUpdate(propertyName);
    }

    private static int BuildActionIdWithUpdate(string propertyName)
    {
        var propertyActionIds = new Dictionary<string, int>
        {
            { "EventDateTime", 2 },
            { "LeagueName", 3 },
            { "HomeName", 4 },
            { "AwayName", 4 },
            { "Tv", 5 },
            { "IPTV", 7 },
            { "Provider", 9 },
            { "Round", 12 },
            { "Remark", 16 },
            { "HomeRemark", 18 },
            { "AwayRemark", 20 },
            { "Division", 22 }
        };

        return propertyActionIds.GetValue(propertyName, 1);
    }

    private static int BuildActionIdWithCancel(string propertyName)
    {
        var propertyActionIds = new Dictionary<string, int>
        {
            { "Tv", 6 },
            { "IPTV", 8 },
            { "Provider", 10 },
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
    {
        return oldValue != string.Empty && newValue == string.Empty;
    }

    public static DateTime DateTimeNowGmt8()
    {
        return DateTime.Now.AddHours(12);
    }

    public static InfoSiteDb CloneInfoSiteMatch(Event source)
    {
        return new InfoSiteDb
        {
            InfoSiteMatchId = source.InfoSiteMatchId,
            LeagueName = source.LeagueName,
            AwayName = source.AwayName,
            HomeName = source.HomeName,
            EventDateTime = source.EventDateTime,
            IPTV = source.Iptv,
            Tv = source.LocalTv,
            Provider = source.Provider,
            AwayRemark = source.AwayRemark,
            Division = source.Division,
            HomeRemark = source.HomeRemark,
            Remark = source.Remark,
            Round = source.Round
        };
    }

    public static IEnumerable<LoggedChangeModel> BuildLoggedChanges(IEnumerable<PropertyInfo> properties, InfoSiteDb infoSiteOld, InfoSiteDb infoSiteNew)
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
                    UpDateTime = DateTimeNowGmt8()
                };
            });
    }
}