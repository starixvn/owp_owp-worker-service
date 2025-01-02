namespace OWP.Adminsite.Constants;

public static class SpecialBettype
{
    public const int BETTYPE_ID_SPECIFIC10MINS = -1;
    public const int BETTYPE_ID_SPECIFIC15MINS = -2;
    public const int BETTYPE_ID_SPECIFIC15MINS_NUMBER_OF_CORNERS = -5;
    public const int BETTYPE_ID_SPECIFIC15MINS_TOTAL_BOOKINGS = -8;

    public static readonly int[] BettypeIdSpecificXMins =
    [
        BETTYPE_ID_SPECIFIC10MINS,
        BETTYPE_ID_SPECIFIC15MINS,
        BETTYPE_ID_SPECIFIC15MINS_NUMBER_OF_CORNERS,
        BETTYPE_ID_SPECIFIC15MINS_TOTAL_BOOKINGS
    ];

    public const int BETTYPE_ID_MAIN_MATCH = 0;
    public const int BETTYPE_ID_NO_OF_CORNERS = -3;
    public const int BETTYPE_ID_NEXT_CORNERS = -4;
    public const int BETTYPE_ID_TOTAL_BOOKINGS = -6;
    public const int BETTYPE_ID_NEXT_BOOKINGS = -7;

    public static readonly int[] BettypeIdSpecialProducts =
    [
        BETTYPE_ID_NO_OF_CORNERS,
        BETTYPE_ID_NEXT_CORNERS,
        BETTYPE_ID_TOTAL_BOOKINGS,
        BETTYPE_ID_NEXT_BOOKINGS
    ];
}