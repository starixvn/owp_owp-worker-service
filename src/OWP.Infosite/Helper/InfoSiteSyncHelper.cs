using OWP.Infosite.Models;

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
        return propertyName switch
        {
            "EventDateTime" => 3,
            "LeagueName" => 4,
            "HomeName" => 5,
            "AwayName" => 6,
            "Tv" => 7,
            "IPTV" => 9,
            "Provider" => 11,
            _ => 1
        };
    }

    private static int BuildActionIdWithCancel(string propertyName)
    {
        return propertyName switch
        {
            "Tv" => 8,
            "IPTV" => 10,
            "Provider" => 12,
            _ => 1
        };
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