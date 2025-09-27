using System.Reflection;
using OWP.AutoGenerateMaxbetSetting.Models;

namespace OWP.AutoGenerateMaxbetSetting.Services;
public interface LeagueSettingChangeService
{
    List<object> LogProcessingChanges(LeagueSettingChangeModel before, LeagueSettingChangeModel after);
}

public class LeagueSettingChangeServiceImp : LeagueSettingChangeService
{
    private static readonly Dictionary<string, string> PropertyDisplayNames = new()
    {
        { "SpreadShow", "FT Spread" },
        { "MaxbetWdHdp", "MB Weekday HDP" },
        { "WDNoOfHDP", "MB Weekday NO.HDP" },
        { "MaxbetWkHdp", "MB Weekend HDP" },
        { "WKNoOfHDP", "MB Weekend NO.HDP" },
    };

    public List<object> LogProcessingChanges(LeagueSettingChangeModel before, LeagueSettingChangeModel after)
    {
        var changes = new List<object>();
        var ignoreList = new[] { nameof(LeagueSettingChangeModel.SpreadHdp), nameof(LeagueSettingChangeModel.SpreadOu) };

        var properties = typeof(LeagueSettingChangeModel)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !ignoreList.Contains(p.Name));

        foreach (var property in properties)
        {
            var beforeValue = property.GetValue(before);
            var afterValue = property.GetValue(after);

            if (!AreValuesEqual(beforeValue, afterValue))
            {
                var displayName = PropertyDisplayNames[property.Name];

                changes.Add(new
                {
                    Key = displayName,
                    OldValue = beforeValue,
                    NewValue = afterValue
                });
            }
        }

        return changes;
    }

    public static bool AreValuesEqual(object value1, object value2)
    {
        if (value1 is string || value2 is string)
        {
            var s1 = value1 as string ?? string.Empty;
            var s2 = value2 as string ?? string.Empty;
            return s1 == s2;
        }

        if (value1 is decimal? || value2 is decimal?)
        {
            var d1 = (value1 as decimal?) ?? 0;
            var d2 = (value2 as decimal?) ?? 0;
            return d1 == d2;
        }

        return Equals(value1, value2);
    }
}