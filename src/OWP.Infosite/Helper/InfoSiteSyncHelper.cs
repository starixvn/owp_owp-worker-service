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
            "EventDateTime" => 2,
            "LeagueName" => 3,
            "HomeName" => 4,
            "AwayName" => 4,
            "Tv" => 5,
            "IPTV" => 7,
            "Provider" => 9,
            "Round" => 12,
            "TeamRound" => 14,
            "Remark"=> 16,
            "HomeRemark" => 18,
            "AwayRemark" => 20,
            "Division"=>22,
            _ => 1
        };
    }

    private static int BuildActionIdWithCancel(string propertyName)
    {
        return propertyName switch
        {
            "Tv" => 6,
            "IPTV" => 8,
            "Provider" => 10,
            "Round" => 13,
            "TeamRound" => 15,
            "Remark" => 17,
            "HomeRemark" => 19,
            "AwayRemark" => 21,
            "Division" => 23,
            _ => 1
        };
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
}