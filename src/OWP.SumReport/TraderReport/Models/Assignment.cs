namespace OWP.SumReport.TraderReport.Models;

using Enum;

public class Assignment
{
    public int UserId { get; set; }

    public string OwpLeagueName { get; set; }

    public int OwpMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public ReportType ReportType { get; set; }
}