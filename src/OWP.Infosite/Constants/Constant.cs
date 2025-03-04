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