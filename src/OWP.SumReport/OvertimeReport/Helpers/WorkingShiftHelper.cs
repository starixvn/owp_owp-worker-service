namespace OWP.SumReport.OvertimeReport.Helpers;

public static class WorkingShiftHelper
{
    public static decimal RoundUpOvertime(decimal result)
    {
        var time = TimeSpan.FromHours((double)result);
        var remainder = time.Minutes;

        return remainder switch
        {
            <= 14 => time.Hours,
            <= 39 => (decimal)(time.Hours + 0.5),
            _ => time.Hours + 1
        };
    }
}