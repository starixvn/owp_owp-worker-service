namespace OWP.SumReport.TraderReport.Models;

public class RobotX5RawGroupModel
{
    public int UserId { get; set; }

    public int OWPMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public int MatchGroup { get; set; }

    public int Minutes { get; set; }

    public int Shift { get; set; }
}