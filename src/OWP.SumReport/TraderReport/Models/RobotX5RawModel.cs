namespace OWP.SumReport.TraderReport.Models;

public class RobotX5RawModel
{
    public int UserId { get; set; }

    public int OwpMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public bool IsFullTime { get; set; }

    public bool Is35MinuteMatch { get; set; }
}