namespace OWP.SumReport.LAReport.Models;

using Enum;

public class Assignment
{
    public int AssignmentId { get; set; }

    public int UserId { get; set; }

    public string ProviderName { get; set; }

    public string OwpLeagueName { get; set; }

    public int? MinorBettypeId { get; set; }

    public int OwpMatchId { get; set; }

    public int? ParentMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public ReportType ReportType { get; set; }

    public int LinkGroup { get; set; }

    public double Count { get; set; }

    public bool SingleAssignment { get; set; }
}