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
        { 1, "Add a new match"},
        { 2, "Update the <b>KO time</b>"},
        { 3, "Update the <b>League name</b>"},
        { 4, "Update the <b>Team name</b>"},
        { 5, "Update the <b>TV</b>"},
        { 6, "Cancel the <b>TV</b>"},
        { 7, "Update the <b>IPTV</b>" },
        { 8, "Cancel the <b>IPTV</b>"},
        { 9, "Update the <b>Other Provider</b>" },
        { 10, "Cancel the <b>Other Provider</b>"},
        { 11, "Delete the match"},
        { 12, "Update the <b>Round</b>"},
        { 13, "Cancel the <b>Round</b>" },
        { 14, "Update the <b>Round (H-A/T)</b>"},
        { 15, "Cancel the <b>Round (H-A/T)</b>" },
        { 16, "Update the <b>Remark</b>"},
        { 17, "Cancel the <b>Remark</b>" },
        { 18, "Update the <b>Home Remark</b>"},
        { 19, "Cancel the <b>Home Remark</b>" },
        { 20, "Update the <b>Away Remark</b>"},
        { 21, "Cancel the <b>Away Remark</b>" },
        { 22, "Update the <b>Division</b>"},
        { 23, "Cancel the <b>Division</b>" },
        { 24, "Move <b>KO Date</b>" },
        { 25, "Renew Match" },
        { 26, "Match Postponed" },
        { 27, "Match Cancelled" },
        { 28, "Update the <b>Round Confirm</b>"},
        { 29, "Cancel the <b>Round Confirm</b>" },
        { 30, "Update the <b>AdminSiteID</b>"},
        { 31, "Cancel the <b>AdminSiteID</b>" },
        { 32, "Update the <b>Web1</b>"},
        { 33, "Cancel the <b>Web1</b>" },
        { 34, "Update the <b>Web2</b>"},
        { 35, "Cancel the <b>Web2</b>" },
        { 36, "Update the <b>Web3</b>"},
        { 37, "Cancel the <b>Web3</b>" },
        { 38, "Update the <b>Web4</b>"},
        { 39, "Cancel the <b>Web4</b>" },
        { 40, "Update the <b>Link</b>"},
        { 41, "Cancel the <b>Link</b>" },
        { 42, "Update the <b>Channel</b>"},
        { 43, "Cancel the <b>Channel</b>" },
        { 44, "Update the <b>Agent</b>"},
        { 45, "Cancel the <b>Agent</b>" },
        { 46, "Update the <b>Venue</b>"},
        { 47, "Cancel the <b>Venue</b>" },
        { 48, "Update the <b>ETPK</b>"},
        { 49, "Cancel the <b>ETPK</b>" },
        { 50, "Update the <b>Neutral</b>"},
        { 51, "Cancel the <b>Neutral</b>" }
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
}