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
        { 9, "Update the <b>Provider</b>" },
        { 10, "Cancel the <b>Provider</b>"},
        { 11, "Delete the match"},
        { 12, "Update the <b>Round</b>"},
        { 13,"Cancel the <b>Round</b>" },
        { 14,"Update the <b>Round (H-A/T)</b>"},
        { 15,"Cancel the <b>Round (H-A/T)</b>" },
        { 16,"Update the <b>Remark</b>"},
        { 17,"Cancel the <b>Remark</b>" },
        { 18,"Update the <b>Home Remark</b>"},
        { 19,"Cancel the <b>Home Remark</b>" },
        { 20,"Update the <b>Away Remark</b>"},
        { 21,"Cancel the <b>Away Remark</b>" },
        { 22,"Update the <b>Division</b>"},
        { 23,"Cancel the <b>Division</b>" },

    }.ToImmutableDictionary();

    public static readonly IImmutableDictionary<int, string> StreamNameSrc2InfoSite = new Dictionary<int, string>
    {
        {0, string.Empty },
        {11, "Glive"}
    }.ToImmutableDictionary();

    public const string DateTimeFormat = "yyyy-MM-dd";

    public const string DateTimeWithHourFormat = "yyyy-MM-dd HH:mm:ss";

    public const string OldPreMatchLeaguesSnapshotKey = "OWP-Old-PreMatchLeagues";

    public const int CacheExpiredIn30Minutes = 30;
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