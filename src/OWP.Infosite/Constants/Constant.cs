using Olympus.OWP.Constants;

namespace OWP.Infosite.Constants;

using System.Collections.Immutable;

public static class Constant
{
    public static readonly IImmutableDictionary<int, string> StreamNameSrc1InfoSite = new Dictionary<int, string>
    {
        {0, string.Empty },
        {4, "Spanish"},
        {8, "IMG"},
        {9, "Watch & Bet"},
        {12, "SportRadar"},
        {13, "In House"},
        {14, "Player"},
        {15, "Betgenius"},
        {17, "LSports"},
        {18, "Weverdat"},
        {19, "CloundRock"},
        {20, "LiveStreamer"}
    }.ToImmutableDictionary();

    public static readonly IImmutableDictionary<int, string> UpdateType = new Dictionary<int, string>
    {
        { UpdateTypeConstant.NEW_MATCH,"Add a new match"},
        { UpdateTypeConstant.UPDATE_KO_TIME,"Update the <b>KO time</b>"},
        { UpdateTypeConstant.UPDATE_LEAGUE,"Update the <b>League name</b>"},
        { UpdateTypeConstant.UPDATE_TEAM,"Update the <b>Team name</b>"},
        { UpdateTypeConstant.UPDATE_TV,"Update the <b>TV</b>"},
        { UpdateTypeConstant.CANCEL_TV,"Cancel the <b>TV</b>"},
        { UpdateTypeConstant.UPDATE_IPTV , "Update the <b>IPTV</b>" },
        { UpdateTypeConstant.CANCEL_IPTV , "Cancel the <b>IPTV</b>"},
        { UpdateTypeConstant.UPDATE_OTHER_PROVIDER, "Update the <b>Other Provider</b>" },
        { UpdateTypeConstant.CANCEL_OTHER_PROVIDER, "Cancel the <b>Other Provider</b>"},
        { UpdateTypeConstant.DELETE_MATCH, "Delete the match"},
        { UpdateTypeConstant.UPDATE_ROUND, "Update the <b>Round</b>"},
        { UpdateTypeConstant.CANCEL_ROUND, "Cancel the <b>Round</b>" },
        { UpdateTypeConstant.UPDATE_ROUND_HAT   , "Update the <b>Round (H-A/T)</b>"},
        { UpdateTypeConstant.CANCEL_ROUND_HAT   , "Cancel the <b>Round (H-A/T)</b>" },
        { UpdateTypeConstant.UPDATE_REMARK    , "Update the <b>Remark</b>"},
        { UpdateTypeConstant.CANCEL_REMARK    , "Cancel the <b>Remark</b>" },
        { UpdateTypeConstant.UPDATE_HOME_REMARK , "Update the <b>Home Remark</b>"},
        { UpdateTypeConstant.CANCEL_HOME_REMARK , "Cancel the <b>Home Remark</b>" },
        { UpdateTypeConstant.UPDATE_AWAY_REMARK , "Update the <b>Away Remark</b>"},
        { UpdateTypeConstant.CANCEL_AWAY_REMARK , "Cancel the <b>Away Remark</b>" },
        { UpdateTypeConstant.UPDATE_DIVISION   , "Update the <b>Division</b>"},
        { UpdateTypeConstant.CANCEL_DIVISION   , "Cancel the <b>Division</b>" },
        { UpdateTypeConstant.MOVE_KICKOFF_DATE  , "Move <b>KO Date</b>" },
        { UpdateTypeConstant.RENEW_MATCH    , "Renew Match" },
        { UpdateTypeConstant.MATCH_POSTPONED   , "Match <b>Postponed</b>" },
        { UpdateTypeConstant.MATCH_CANCELLED   , "Match <b>Cancelled</b>" },
        { UpdateTypeConstant.UPDATE_ROUND_CONFIRM, "Update the <b>Round Confirm</b>"},
        { UpdateTypeConstant.CANCEL_ROUND_CONFIRM, "Cancel the <b>Round Confirm</b>" },
        { UpdateTypeConstant.UPDATE_ADMINSITEID , "Update the <b>AdminSiteID</b>"},
        { UpdateTypeConstant.CANCEL_ADMINSITEID , "Cancel the <b>AdminSiteID</b>" },
        { UpdateTypeConstant.UPDATE_WEB1      , "Update the <b>Web1</b>"},
        { UpdateTypeConstant.CANCEL_WEB1      , "Cancel the <b>Web1</b>" },
        { UpdateTypeConstant.UPDATE_WEB2      , "Update the <b>Web2</b>"},
        { UpdateTypeConstant.CANCEL_WEB2      , "Cancel the <b>Web2</b>" },
        { UpdateTypeConstant.UPDATE_WEB3      , "Update the <b>Web3</b>"},
        { UpdateTypeConstant.CANCEL_WEB3      , "Cancel the <b>Web3</b>" },
        { UpdateTypeConstant.UPDATE_WEB4      , "Update the <b>Web4</b>"},
        { UpdateTypeConstant.CANCEL_WEB4      , "Cancel the <b>Web4</b>" },
        { UpdateTypeConstant.UPDATE_LINK      , "Update the <b>Link</b>"},
        { UpdateTypeConstant.CANCEL_LINK      , "Cancel the <b>Link</b>" },
        { UpdateTypeConstant.UPDATE_CHANNEL    , "Update the <b>Channel</b>"},
        { UpdateTypeConstant.CANCEL_CHANNEL    , "Cancel the <b>Channel</b>" },
        { UpdateTypeConstant.UPDATE_AGENT  , "Update the <b>Agent</b>"},
        { UpdateTypeConstant.CANCEL_AGENT  , "Cancel the <b>Agent</b>" },
        { UpdateTypeConstant.UPDATE_VENUE  , "Update the <b>Venue</b>"},
        { UpdateTypeConstant.CANCEL_VENUE  , "Cancel the <b>Venue</b>" },
        { UpdateTypeConstant.UPDATE_ETPK   , "Update the <b>ETPK</b>"},
        { UpdateTypeConstant.CANCEL_ETPK   , "Cancel the <b>ETPK</b>" },
        { UpdateTypeConstant.UPDATE_NEUTRAL   , "Update the <b>Neutral</b>"},
        { UpdateTypeConstant.CANCEL_NEUTRAL   , "Cancel the <b>Neutral</b>" },
        { UpdateTypeConstant.MATCH_SUSPENDED    , "Match <b>Suspended</b>" },
        { UpdateTypeConstant.UPDATE_REMARK2, "Update the <b>Remark2</b>" },
        { UpdateTypeConstant.CANCEL_REMARK2, "Cancel the <b>Remark2</b>" },
        { 55, "Update the <b>Provider</b>" },
        { 56, "Cancel the <b>Provider</b>" }
    }.ToImmutableDictionary();

    public static readonly IImmutableDictionary<int, string> StreamNameSrc2InfoSite = new Dictionary<int, string>
    {
        {0, string.Empty },
        {11, "Glive"}
    }.ToImmutableDictionary();

    public static readonly IImmutableDictionary<int, string> WebsiteProvider = new Dictionary<int, string>
    {
        { 3, "365GV"},
        { 15, "IMG"},
        { 17, "RB2" },
        { 18, "RTS" },
        { 19, "BG" },
        { 20, "BET" }
    }.ToImmutableDictionary();

    public static readonly IImmutableDictionary<int, string> CoveragePair = new Dictionary<int, string>
    {
        { 0, string.Empty },
        { 1, "TV" },
        { 2, "TV(L)" }
    }.ToImmutableDictionary();

    public const string DateTimeFormat = "yyyy-MM-dd";

    public const string DateTimeWithHourFormat = "yyyy-MM-dd HH:mm:ss";

    public const string OldPreMatchLeaguesSnapshotKey = "OWP-Old-PreMatchLeagues";
}

//ChildMatch
public static class MinorBettypeId
{
    public const int NextCorner = -4;
    public const int NextBooking = -7;
    public const int Specific15Min = -1;
    public const int PenaltyToScore = -96;
    public const int PenaltyAwarded = -97;
    public const int NextGoalKick = -100;
}

public static class InfoSiteMatchStatus
{
    public const byte Normal = 0;
    public const byte Postponed = 1;
    public const byte Cancelled = 2;
    public const byte Suspended = 4;
}