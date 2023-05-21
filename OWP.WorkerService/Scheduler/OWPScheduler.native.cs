namespace OWP.WorkerService.Scheduler;

using Constants;
using Jobs;
using Models;
using Quartz;
using Quartz.Spi;

public class OwpScheduler : IHostedService
{
    private readonly ISchedulerFactory schedulerFactory;
    private readonly IJobFactory jobFactory;

    public IScheduler Scheduler { get; set; }

    public OwpScheduler(ISchedulerFactory schedulerFactory, IJobFactory jobFactory)
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
        //InfositeMatch
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncTodayInfoSiteMatchJob), CronExpressionJob.SyncTodayInfoSiteMatch));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncFutureInfoSiteMatchJob), CronExpressionJob.SyncFutureInfoSiteMatch));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncEventDeletedJob), CronExpressionJob.SyncEventDeleted));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncInfositeIdJob), CronExpressionJob.SyncInfositeId));
        await AddJobToSchedulerAsync(new JobInfo(typeof(Sync180DayInfositeMatchJob), CronExpressionJob.Sync180DayInfoSiteMatch));

        //InfositeChildMatch
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncTodayInfoSiteChildMatchJob), CronExpressionJob.SyncTodayInfoSiteMatch));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncFutureInfoSiteChildMatchJob), CronExpressionJob.SyncFutureInfoSiteMatch));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncDeletedChildInfoSiteMatchJob), CronExpressionJob.SyncEventDeleted));

        //Report
        await AddJobToSchedulerAsync(new JobInfo(typeof(SumTraderReportJob), CronExpressionJob.SumDataReport));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SumLaReportJob), CronExpressionJob.SumDataReport));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SumOvertimeReportJob), CronExpressionJob.SumDataReport));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SumPrematchReportJob), CronExpressionJob.SumDataReport));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SumMatchLiveReportJob), CronExpressionJob.SumDataReport));

        //OtherSport
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncOtherSportAdminSiteMatchJob), CronExpressionJob.SyncOtherSportAdminSiteJob));

        //Others
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncDataNotificationSettingJob), CronExpressionJob.SyncDataNotificationSetting));
        await AddJobToSchedulerAsync(new JobInfo(typeof(MatchReadyNotificationJob), CronExpressionJob.MatchReadyNotification));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncPreMatchLeagueJob), CronExpressionJob.SyncPreMatchLeague));
        await AddJobToSchedulerAsync(new JobInfo(typeof(SyncLeagueSettingsJob), CronExpressionJob.SyncLeagueSetting));
        await AddJobToSchedulerAsync(new JobInfo(typeof(LinkLeagueAndTeamJob), CronExpressionJob.LinkLeagueAndMatch));
        await AddJobToSchedulerAsync(new JobInfo(typeof(DeleteOldMessageJob), CronExpressionJob.DeleteOldMessage));
        await AddJobToSchedulerAsync(new JobInfo(typeof(ArchiveDataJob), CronExpressionJob.ArchiveData));
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