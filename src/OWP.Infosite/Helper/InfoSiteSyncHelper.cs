using OWP.Infosite.Models;

namespace OWP.Infosite.Helper;
public static class InfoSiteSyncHelper
{
    public static int BuildActionId(string propertyName, string newValue, string oldValue)
    {
        if (IsUpDate(oldValue, newValue))
        {
            switch (propertyName)
            {
                case "Tv":
                    return 7;
                case "IPTV":
                    return 9;
                case "Provider":
                    return 11;
            }
        }
        if (IsCancel(oldValue, newValue))
        {
            switch (propertyName)
            {
                case "Tv":
                    return 8;
                case "IPTV":
                    return 10;
                case "Provider":
                    return 12;
            }
        }
        return propertyName switch
        {
            "EventDateTime" => 3,
            "LeagueName" => 4,
            "HomeName" => 5,
            "AwayName" => 6,
            _ => 1
        };
    }

    private static bool IsUpDate(string oldValue, string newValue)
    {
        return (oldValue == string.Empty && newValue != string.Empty) ||
               (newValue != string.Empty && newValue != string.Empty);
    }

    private static bool IsCancel(string oldValue, string newValue)
    {
        return oldValue != string.Empty && newValue == string.Empty;
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
            Provider = source.Provider
        };
    }
}