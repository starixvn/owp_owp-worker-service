using System.Collections.Immutable;

namespace OWP.Infosite.Constants;

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
        {11, "Glive"},
    }.ToImmutableDictionary();

    public const string DateTimeFormat = "yyyy-MM-dd";

    public const string DateTimeWithHourFormat = "yyyy-MM-dd HH:mm:ss";

    public const string OldInfositeMatchesSnapshotKey = "OWP-Old-InfoSiteMatches";

    public const string OldPreMatchLeaguesSnapshotKey = "OWP-Old-PreMatchLeagues";

    public const int CacheExpiredIn10Minutes = 10;

    // Mics Match
    public const int CornerType = 1;
    public const int BookingType = 2;
    public const int Special15MinsType = 3;
    public static readonly IImmutableDictionary<int, string> SpecialMatchDict = new Dictionary<int, string>
    {
        { CornerType, "Next Corners" },
        { BookingType, "Next Booking" },
        { Special15MinsType, "(SPECIFIC 15 MINS)" }
    }.ToImmutableDictionary();
    public static readonly ImmutableList<string> Special15Min = new List<string>
        {"00:00-15:00", "15:01-30:00", "30:01-45:00", "45:01-60:00", "60:01-75:00", "75:01-90:00"}.ToImmutableList();

    public const int NextCornerBettypeId = -4;
    public const int NextBookingBettypeId = -7;
    public const int Specific15MinBettypeId = -1;
}