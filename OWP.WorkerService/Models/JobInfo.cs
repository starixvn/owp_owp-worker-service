namespace OWP.WorkerService.Models;

public class JobInfo(Type type, string cronExpression)
{
    public Type Type { get; set; } = type;
    public string CronExpression { get; set; } = cronExpression;
}