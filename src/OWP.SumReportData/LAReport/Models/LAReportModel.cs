namespace OWP.SumReportData.LAReport.Models;

public class LAReportModel
{
    public int UserId { get; set; }

    public string Username { get; set; }

    public int Link2 { get; set; }

    public int Link3 { get; set; }

    public int Link4 { get; set; }

    public int Link5 { get; set; }

    public int Link6 { get; set; }

    public int Link7 { get; set; }

    public int Link8Plus { get; set; }

    public int OnlyProvider { get; set; }

    public int OnlyTV { get; set; }

    public int OnlyReporter { get; set; }

    public int MixedProviderTVReporter { get; set; }

    public int CornerPlus { get; set; }

    public int TBookingPlus { get; set; }

    public int Fantasy { get; set; }

    public int GoalKick { get; set; }

    public int OtherProducts { get; set; }

    public int FTCorner { get; set; }

    public int ESoccer5Mins { get; set; }

    public int ESoccer6Mins { get; set; }

    public int ESoccer15Mins { get; set; }

    public int ESoccer16Mins { get; set; }

    public int ESoccer20Mins { get; set; }

    public int Others { get; set; }

    public int FiveMajorLeague { get; set; }

    public int FTCorner5ML { get; set; }

    public string OtherProductMatchIds { get; set; } = string.Empty;

    public int TotalMatches =>
        Link2 +
        Link3 +
        Link4 +
        Link5 +
        Link6 +
        Link7 +
        Link8Plus +
        MixedProviderTVReporter +
        OnlyProvider +
        OnlyTV +
        OnlyReporter +
        CornerPlus +
        TBookingPlus +
        Fantasy +
        GoalKick +
        OtherProducts +
        FTCorner +
        ESoccer5Mins +
        ESoccer6Mins +
        ESoccer15Mins +
        ESoccer16Mins +
        ESoccer20Mins +
        Others +
        FiveMajorLeague +
        FTCorner5ML;
}