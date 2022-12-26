namespace OWP.SumReport.PrematchReport.Models;

public record DataReportBase
{
    public int UserId { get; init; }

    public DateTime EventDate { get; init; }
}