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
                    return 6;
                case "IPTV":
                    return 8;
                case "Provider":
                    return 10;
            }
        }
        if (IsCancel(oldValue, newValue))
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
        return propertyName switch
        {
            "DateTime" => 2,
            "LeagueName" => 3,
            "HomeName" => 4,
            "AwayName" => 5,
            _ => 0
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