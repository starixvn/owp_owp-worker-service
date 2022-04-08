namespace OWP.SumReportData.LAReport.Models;

public class LAReportModel
{
    public int UserId { get; set; }

    public string Username { get; set; }

    public int TotalMatches { get; set; }

    public int LinkLowerThan3 { get; set; }

    public int LinkX3 { get; set; }

    public int LinkX6 { get; set; }

    public int LinkX9Plus { get; set; }

    public int ProviderTv { get; set; }

    public int Provider { get; set; }

    public int TV { get; set; }

    public int Reporter { get; set; }

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

    public int SumTotal()
        => LinkLowerThan3 +
        LinkX3 +
        LinkX6 +
        LinkX9Plus +
        ProviderTv +
        Provider +
        TV +
        Reporter +
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
        FiveMajorLeague;
}