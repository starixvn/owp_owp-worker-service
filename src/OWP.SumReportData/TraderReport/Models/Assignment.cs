using OWP.SumReportData.TraderReport.Enum;

namespace OWP.SumReportData.TraderReport.Models;

public class Assignment
{
    public int UserId { get; set; }

    public string OWPLeagueName { get; set; }

    public int OWPMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public ReportType ReportType { get; set; }
}