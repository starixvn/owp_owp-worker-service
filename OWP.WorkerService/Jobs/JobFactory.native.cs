namespace OWP.WorkerService.Jobs;

using Quartz;
using Quartz.Spi;

public class JobFactory : IJobFactory
{
    private readonly IServiceProvider serviceProvider;

    public JobFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var jobDetail = bundle.JobDetail;
        return serviceProvider.GetService(jobDetail.JobType) as IJob;
    }

    public void ReturnJob(IJob job)
    {
    }
}