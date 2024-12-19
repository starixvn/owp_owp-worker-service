namespace OWP.WorkerService.Jobs;

using Quartz;
using Quartz.Spi;

public class JobFactory(IServiceProvider serviceProvider) : IJobFactory
{
    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var jobDetail = bundle.JobDetail;
        return (IJob)serviceProvider.GetService(jobDetail.JobType) ?? new EmptyJob();
    }

    public void ReturnJob(IJob job)
    {
    }
}

public class EmptyJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await Task.CompletedTask;
    }
}