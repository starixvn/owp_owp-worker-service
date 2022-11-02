namespace OWP.SumReport.OvertimeReport.Models;

public class Assignment
{
    public int UserId { get; set; }

    public int OWPMatchId { get; set; }

    public int? ParentMatchId { get; set; }

    public int? MinorBettypeId { get; set; }

    public DateTime EventDateTime { get; set; }

    public int Bettype { get; set; }

    public string Remark { get; set; }

    public string OWPLeagueName { get; set; }
}