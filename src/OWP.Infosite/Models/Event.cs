namespace OWP.Infosite.Models;

using Builder;
using Extensions;
using Newtonsoft.Json;
using Olympus.Xml;
using System.Xml.Serialization;

[Serializable]
public class Event
{
    private const int SoccerSportId = 1;
    private static readonly DateTime MinDate = new(2000, 1, 1);
    private readonly CommonMatchModel infositeMatch;
    private readonly SoccerEventSchedule extInfo;

    public int AdminSiteMatchId { get; private set; }

    public int InfoSiteMatchId { get; private set; }

    public bool IsLive { get; set; }

    public string LeagueName { get; private set; }

    public string HomeName { get; private set; }

    public string AwayName { get; private set; }

    public string Round { get; private set; }

    public string TeamRound { get; set; }

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

    [XmlName("ProviderTV")]
    public string ProviderTv { get; set; }

    public string ProviderUsed { get; set; }

    public DateTime EventDateTime { get; private set; }

    [XmlName("LocalTV")]
    public string LocalTv { get; private set; }

    [XmlName("LiveTV")]
    public string LiveTv { get; private set; }

    public string Iptv { get; private set; }

    public DateTime UpdatedTime { get; private set; }

    public bool IsNeutral { get; set; }

    public int SportId { get; set; }

    [XmlName("ETPK")]
    public string Etpk { get; set; }

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

    public bool IsLeagueBlackList { get; set; }

    public bool IsHomeBlackList { get; set; }

    public bool IsAwayBlackList { get; set; }

    public Event(CommonMatchModel infositeMatch)
    {
        this.infositeMatch = infositeMatch;
        extInfo = infositeMatch.ext_info ?? new SoccerEventSchedule();
        InfoSiteMatchId = infositeMatch.match_id;
        AdminSiteMatchId = extInfo.create_match_id;
        IsLive = extInfo.create_is_live.HasValue && extInfo.create_is_live.Value;
        LeagueName = infositeMatch.league_name;
        HomeName = GetHomeName();
        AwayName = GetAwayName();
        Round = extInfo.round.ConvertNullToEmpty();
        Division = extInfo.code.ConvertNullToEmpty();
        Agent = extInfo.agent.ConvertNullToEmpty();
        Venue = extInfo.venue.ConvertNullToEmpty();
        WatchBet = WatchBetBuilder.Build(extInfo);
        LocalTv = extInfo.local_tv.ConvertNullToEmpty();
        LiveTv = extInfo.tv_live.ConvertNullToEmpty();
        Iptv = extInfo.ip_tv.ConvertNullToEmpty();
        Website1 = extInfo.website1.ConvertNullToEmpty();
        Website2 = extInfo.website2.ConvertNullToEmpty();
        Website3 = extInfo.website3.ConvertNullToEmpty();
        Website4 = extInfo.website4.ConvertNullToEmpty();
        Provider = extInfo.provider.FormatProviders();
        OtherProvider = OtherProviderBuilder.Build(infositeMatch);
        ProviderTv = infositeMatch.TvBuild();
        ProviderUsed = extInfo.provider_use.ConvertNullToEmpty();
        Etpk = extInfo.et_pk.ConvertNullToEmpty();
        UpdatedTime = GetUpdateTime();
        EventDateTime = GetEventDateTime();
        IsNeutral = extInfo.is_neutral.HasValue && extInfo.is_neutral.Value;
        SportId = extInfo.sport_id;
        IsValid = IsValidMatch();
        MatchCode = GetMatchCode();
        Remark = extInfo.remark;
        IsTest = infositeMatch.is_test;
        Remark2 = extInfo.remark2.ConvertNullToEmpty();
        HomeRemark = extInfo.home_remark.ConvertNullToEmpty();
        AwayRemark = extInfo.away_remark.ConvertNullToEmpty();
        TotalRound = extInfo.round_total;
        IsLeagueBlackList = infositeMatch.is_league_black;
        IsHomeBlackList = infositeMatch.is_home_black;
        IsAwayBlackList = infositeMatch.is_away_black;
    }

    private string GetHomeName()
    {
        var isReverse = extInfo.is_team_reverse.HasValue && extInfo.is_team_reverse.Value;
        var homeName = isReverse ? infositeMatch.away_name : infositeMatch.home_name;
        return homeName;
    }

    private string GetAwayName()
    {
        var isReverse = extInfo.is_team_reverse.HasValue && extInfo.is_team_reverse.Value;
        return isReverse ? infositeMatch.home_name : infositeMatch.away_name;
    }

    private DateTime GetEventDateTime()
    {
        var exInfoGameDate = infositeMatch.ext_info?.game_date ?? MinDate;

        return exInfoGameDate.ToUtc8();
    }

    private DateTime GetUpdateTime()
    {
        var exInfoTime = extInfo.update_time ?? MinDate;
        var matchTime = infositeMatch.update_timestamp ?? MinDate;
        matchTime = matchTime.ToUtc0();
        return exInfoTime > matchTime ? exInfoTime : matchTime;
    }

    private string GetMatchCode()
        => infositeMatch.league_name.Contains("SABA", StringComparison.OrdinalIgnoreCase)
            ? extInfo.create_match_code.ConvertNullToEmpty()
            : string.Empty;

    private bool IsValidMatch()
        => IsValidExtInfoMatch() && IsConfirmMatch();

    private bool IsValidExtInfoMatch()
        => infositeMatch.ext_info is { rd_checked: { } }
           && infositeMatch.ext_info.rd_checked.Value
           && extInfo.sport_id == SoccerSportId;

    private bool IsConfirmMatch()
        => !(infositeMatch.ibc_league_id == null
            && infositeMatch.ibc_home_id == null
            && infositeMatch.ibc_away_id == null);
}