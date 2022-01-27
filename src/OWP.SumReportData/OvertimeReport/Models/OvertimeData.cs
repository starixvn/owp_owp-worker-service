namespace OWP.SumReportData.OvertimeReport.Models;

public class OvertimeData
{
    public int UserId { get; set; }

    public DateTime ReportDate { get; set; }

    public decimal Hour { get; set; }

    public bool IsOffDay { get; set; }
}