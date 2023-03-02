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

    public const string DateTimeFormat = "yyyy-MM-dd";

    public const string DateTimeWithHourFormat = "yyyy-MM-dd HH:mm:ss";

    public const string OldInfositeMatchesSnapshotKey = "OWP-Old-InfoSiteMatches";

    public const string OldPreMatchLeaguesSnapshotKey = "OWP-Old-PreMatchLeagues";

    public const int CacheExpiredIn10Minutes = 10;
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