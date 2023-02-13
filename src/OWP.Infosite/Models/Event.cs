using Newtonsoft.Json;
using OWP.Attributes;
using OWP.Extensions;
using OWP.Infosite.Builder;
using System.Xml.Serialization;

namespace OWP.Infosite.Models;

[Serializable]
[XmlName("Row")]
public class Event
{
    private const int SoccerSportId = 1;
    private static readonly DateTime MinDate = new DateTime(2000, 1, 1);
    private readonly CommonMatchModel InfositeMatch;
    private readonly SoccerEventSchedule ExtInfo;

    public int AdminSiteMatchId { get; private set; }

    public int InfoSiteMatchId { get; private set; }

    public bool IsLive { get; set; }

    public string LeagueName { get; private set; }

    public string HomeName { get; private set; }

    public string AwayName { get; private set; }

    public string Round { get; private set; }

    public string Division { get; private set; }

    public string Agent { get; private set; }

    public string Venue { get; private set; }

    public string WatchBet { get; private set; }

    public string Website1 { get; private set; }

    public string Website2 { get; private set; }

    public string Website3 { get; private set; }

    public string Website4 { get; private set; }

    public string Provider { get; set; }

    public string OtherProvider { get; set; }

    public string ProviderTV { get; set; }

    public string ProviderUsed { get; set; }

    public DateTime EventDateTime { get; private set; }

    public string LocalTV { get; private set; }

    public string LiveTV { get; private set; }

    public string Iptv { get; private set; }

    public DateTime UpdatedTime { get; private set; }

    public bool IsNeutral { get; set; }

    public int SportId { get; set; }

    public string ETPK { get; set; }

    public string Remark { get; set; }

    [JsonIgnore]
    [XmlIgnore]
    public bool IsValid { get; private set; }

    public string MatchCode { get; private set; }

    public bool IsTest { get; set; }

    public string Remark2 { get; set; }

    public string HomeRemark { get; set; }

    public string AwayRemark { get; set; }

    public string TotalRound { get; set; }

    public Event(CommonMatchModel infositeMatch)
    {
        InfositeMatch = infositeMatch;
        ExtInfo = infositeMatch.ext_info ?? new SoccerEventSchedule();
        InfoSiteMatchId = infositeMatch.match_id;
        AdminSiteMatchId = ExtInfo.create_match_id;
        IsLive = ExtInfo.create_is_live.HasValue && ExtInfo.create_is_live.Value;
        LeagueName = infositeMatch.league_name;
        HomeName = GetHomeName();
        AwayName = GetAwayName();
        Round = ExtInfo.round.ConvertNullToEmpty();
        Division = ExtInfo.code.ConvertNullToEmpty();
        Agent = ExtInfo.agent.ConvertNullToEmpty();
        Venue = ExtInfo.venue.ConvertNullToEmpty();
        WatchBet = WatchBetBuilder.Build(ExtInfo);
        LocalTV = ExtInfo.local_tv.ConvertNullToEmpty();
        LiveTV = ExtInfo.tv_live.ConvertNullToEmpty();
        Iptv = ExtInfo.ip_tv.ConvertNullToEmpty();
        Website1 = ExtInfo.website1.ConvertNullToEmpty();
        Website2 = ExtInfo.website2.ConvertNullToEmpty();
        Website3 = ExtInfo.website3.ConvertNullToEmpty();
        Website4 = ExtInfo.website4.ConvertNullToEmpty();
        Provider = ExtInfo.provider.FormatProviders();
        OtherProvider = OtherProviderBuilder.Build(infositeMatch);
        ProviderTV = infositeMatch.TvBuild();
        ProviderUsed = ExtInfo.provider_use.ConvertNullToEmpty();
        ETPK = ExtInfo.et_pk.ConvertNullToEmpty();
        UpdatedTime = GetUpdateTime();
        EventDateTime = GetEventDateTime();
        IsNeutral = ExtInfo.is_neutral.HasValue && ExtInfo.is_neutral.Value;
        SportId = ExtInfo.sport_id;
        IsValid = IsValidMatch();
        MatchCode = GetMatchCode();
        Remark = ExtInfo.remark;
        IsTest = infositeMatch.is_test;
        Remark2 = ExtInfo.remark2.ConvertNullToEmpty();
        HomeRemark = ExtInfo.home_remark.ConvertNullToEmpty();
        AwayRemark = ExtInfo.away_remark.ConvertNullToEmpty();
        TotalRound = ExtInfo.round_total;
    }

    private string GetHomeName()
    {
        var isReverse = ExtInfo.is_team_reverse.HasValue && ExtInfo.is_team_reverse.Value;
        var homename = isReverse ? InfositeMatch.away_name : InfositeMatch.home_name;
        return homename;
    }

    private string GetAwayName()
    {
        var isReverse = ExtInfo.is_team_reverse.HasValue && ExtInfo.is_team_reverse.Value;
        return isReverse ? InfositeMatch.home_name : InfositeMatch.away_name;
    }

    private DateTime GetEventDateTime()
    {
        var exInfoGameDate = InfositeMatch.ext_info?.game_date != null && InfositeMatch.ext_info.game_date.HasValue
            ? InfositeMatch.ext_info.game_date.Value
            : MinDate;

        return exInfoGameDate.ToUtc8();
    }

    private DateTime GetUpdateTime()
    {
        var exInfoTime = ExtInfo.update_time.HasValue ? ExtInfo.update_time.Value : MinDate;
        var matchTime = InfositeMatch.update_timestamp.HasValue ? InfositeMatch.update_timestamp.Value : MinDate;
        matchTime = matchTime.ToUtc0();
        return exInfoTime > matchTime ? exInfoTime : matchTime;
    }

    private string GetMatchCode()
        => InfositeMatch.league_name.Contains("SABA", StringComparison.OrdinalIgnoreCase)
            ? ExtInfo.create_match_code.ConvertNullToEmpty()
            : string.Empty;

    private bool IsValidMatch()
        => IsValidExtInfoMatch() && IsConfirmMatch();

    private bool IsValidExtInfoMatch()
        => InfositeMatch.ext_info != null
            && InfositeMatch.ext_info.rd_checked.HasValue
            && InfositeMatch.ext_info.rd_checked.Value
            && ExtInfo.sport_id == SoccerSportId;

    private bool IsConfirmMatch()
        => !(InfositeMatch.ibc_league_id == null
            && InfositeMatch.ibc_home_id == null
            && InfositeMatch.ibc_away_id == null);
}