namespace OWP.WorkerService.Models;

public class JobSettings
{
    public string Name { get; set; }
    public string CronExpression { get; set; }
    public bool Enable { get; set; }
}