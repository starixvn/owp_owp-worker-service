using OWP.SumReportData.LAReport.Enum;

namespace OWP.SumReportData.LAReport.Models;

public class Assignment
{
    public int AssignmentId { get; set; }

    public int UserId { get; set; }

    public string ProviderName { get; set; }

    public string OWPLeagueName { get; set; }

    public int? MinorBettypeId { get; set; }

    public int OWPMatchId { get; set; }

    public int? ParentMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public ReportType ReportType { get; set; }

    public int LinkGroup { get; set; }
}