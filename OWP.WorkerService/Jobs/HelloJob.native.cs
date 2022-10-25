using Quartz;
using static System.Console;

namespace OWP.WorkerService.Jobs
{
    [DisallowConcurrentExecution]
    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            WriteLine($"{DateTime.Now.ToLongDateString()} Hello");
            await Task.CompletedTask;
        }
    }
}