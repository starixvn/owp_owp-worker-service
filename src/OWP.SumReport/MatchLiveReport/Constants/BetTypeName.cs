namespace OWP.SumReport.MatchLiveReport.Constants;

using System.ComponentModel.DataAnnotations;

public enum BetTypeName
{
    [Display(Name = "Main")]
    Main,

    [Display(Name = "Corners")]
    Corners,

    [Display(Name = "Next corners")]
    NextCorners,

    [Display(Name = "Last corners")]
    LastCorners,

    [Display(Name = "15 mins corners")]
    Specific15MinsNumberOfCorners,

    [Display(Name = "15 mins (4 quarters)")]
    Specific15Mins,

    [Display(Name = "10 mins (6 quarters)")]
    Specific10Mins,

    [Display(Name = "Bookings")]
    Bookings,

    [Display(Name = "Next bookings")]
    NextBookings,

    [Display(Name = "15 mins bookings")]
    Specific15MinsTotalBookings,

    [Display(Name = "Last bookings")]
    LastBookings,

    [Display(Name = "Winner")]
    Winner,

    [Display(Name = "Special goal")]
    SpecialGoal,

    [Display(Name = "Single Team Home/Away (2 events)")]
    SingleTeamOu,

    [Display(Name = "Fantasy Match")]
    Fantasy,

    [Display(Name = "1st Penalty To Score")]
    FirstPenaltyToScore,

    [Display(Name = "1st Penalty Awarded")]
    FirstPenaltyAwarded,

    [Display(Name = "Free kick (2H 1st xxx)")]
    FreeKick,

    [Display(Name = "Last Free kick")]
    LastFreeKick,

    [Display(Name = "Throw In (2H 1st xxx)")]
    ThrowIn,

    [Display(Name = "Last Throw In")]
    LastThrowIn,

    [Display(Name = "Offside (2H 1st xxx)")]
    Offside,

    [Display(Name = "Goal kick (2H 1st xxx)")]
    GoalKick,

    [Display(Name = "Next Goal kick")]
    NextGoalKick,

    [Display(Name = "Last Goal kick")]
    LastGoalKick,

    [Display(Name = "Penalty shoot out Home/Away (2 events)")]
    PenaltyShootOut,

    [Display(Name = "Which team to take the first penalty")]
    WhichTeamToTakeTheFirstPenalty,
    
    [Display(Name = "Which team to kick off (ET)")]
    WhichTeamToKickOff,

    [Display(Name = "SABA CLUB FRIENDLY Virtual PES 21 - 15 Mins Play")]
    SabaClubFriendlyVirtualPes2115MinsPlay,
    
    [Display(Name = "SABA INTERNATIONAL FRIENDLY Virtual PES 21 - 20 Mins Play")]
    SabaInternationalFriendlyVirtualPes2120MinsPlay,

    [Display(Name = "SABA ELITE FRIENDLY Virtual PES 21 - 5 Mins Play")]
    SabaEliteFriendlyVirtualPes215MinsPlay,

    [Display(Name = "M88 SABA SPANISH LEAGUE Virtual FIFA 21 Season 10 - 16 Mins Play")]
    M88SabaSpanishLeagueVirtualFifa21Season1016MinsPlay,

    [Display(Name = "SABA CLUB FRIENDLY Virtual FIFA 20 FUTSAL - 6 Mins Play")]
    SabaClubFriendlyVirtualFifa20Futsal6MinsPlay,

    [Display(Name = "SABA CLUB FRIENDLY Virtual FIFA 21 - 16 Mins Play")]
    SabaClubFriendlyVirtualFifa2116MinsPlay,
    
    Other

}