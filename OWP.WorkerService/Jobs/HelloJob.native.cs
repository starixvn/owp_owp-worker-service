using static System.Console;

namespace OWP.WorkerService.Jobs;

using Quartz;

[DisallowConcurrentExecution]
public class HelloJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        WriteLine($"{DateTime.Now.ToLongDateString()} Hello");
        await Task.CompletedTask;
    }
}