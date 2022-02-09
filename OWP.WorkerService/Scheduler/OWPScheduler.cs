using OWP.WorkerService.Constants;
using OWP.WorkerService.Jobs;
using OWP.WorkerService.Models;
using Quartz;
using Quartz.Spi;

namespace OWP.WorkerService.Scheduler;

public class OWPScheduler : IHostedService
{
    private readonly ISchedulerFactory schedulerFactory;
    private readonly IJobFactory jobFactory;

    public IScheduler Scheduler { get; set; }

    public OWPScheduler(ISchedulerFactory schedulerFactory, IJobFactory jobFactory)
    {
        this.schedulerFactory = schedulerFactory;
        this.jobFactory = jobFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await CreateScheduler();
        await RunJobSchedulerAsync();
        await StartScheduler(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Scheduler?.Shutdown(cancellationToken)!;
    }

    private async Task RunJobSchedulerAsync()
    {
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncFutureInfoSiteMatchJob), CronExpressionJob.SyncFutureInfoSiteMatch));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncEventDeletedJob), CronExpressionJob.SyncEventDeleted));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncDataNotificationSettingJob), CronExpressionJob.SyncDataNotificationSetting));
        await AddJobToSchedulerAsync(new JobInfo(typeof(MatchReadyNotificationJob), CronExpressionJob.MatchReadyNotification));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SumTraderReportJob), CronExpressionJob.SumOvertimeReport));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SumLAReportJob), CronExpressionJob.SumOvertimeReport));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncInfositeIdJob), CronExpressionJob.SyncInfositeId));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncPreMatchLeagueJob), CronExpressionJob.SyncPreMatchLeague));
        await AddJobToSchedulerAsync(new JobInfo(typeof(Sync180DayInfositeMatchJob), CronExpressionJob.Sync180DayInfoSiteMatch));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncLeagueSettingsJob), CronExpressionJob.SyncLeagueSetting));
        await AddJobToSchedulerAsync(new JobInfo(typeof(LinkLeagueAndTeamJob), CronExpressionJob.LinkLeagueAndMatch));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SumOvertimeReportJob), CronExpressionJob.SumOvertimeReport));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncTodayInfoSiteMatchJob), CronExpressionJob.SyncTodayInfoSiteMatch));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncOtherSportAdminSiteMatchJob), CronExpressionJob.SyncOtherSportAdminSiteJob));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SumPrematchReportJob), CronExpressionJob.PrematchReport));
    }

    private async Task CreateScheduler()
    {
        Scheduler = await schedulerFactory.GetScheduler();
        Scheduler.JobFactory = jobFactory;
    }

    private async Task AddJobToSchedulerAsync(JobInfo jobInfo)
    {
        var jobDetail = CreateJob(jobInfo);

        var trigger = CreateTrigger(jobInfo);

        await Scheduler.ScheduleJob(jobDetail, trigger);
    }

    private async Task StartScheduler(CancellationToken cancellationToken)
    {
        await Scheduler.Start(cancellationToken);
    }

    private static ITrigger CreateTrigger(JobInfo jobInfo)
        => TriggerBuilder
            .Create()
            .WithIdentity($"trigger{jobInfo.Type.Name}", $"group{jobInfo.Type.Name}")
            .WithDescription(jobInfo.Type.Name)
            .WithCronSchedule(jobInfo.CronExpression)
            .Build();

    private static IJobDetail CreateJob(JobInfo jobInfo)
        => JobBuilder
            .Create(jobInfo.Type)
            .WithIdentity($"sync{jobInfo.Type.Name}", $"group{jobInfo.Type.Name}")
            .Build();
}