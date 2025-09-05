namespace OWP.WorkerService.Scheduler;

using Jobs;
using Models;
using Quartz;
using Quartz.Spi;

public class OwpScheduler(
    ISchedulerFactory schedulerFactory,
    IJobFactory jobFactory,
    List<JobSettings> jobSettings) : IHostedService
{
    public IScheduler Scheduler { get; set; }

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
        await AddJobToSchedulerAsync(typeof(SyncTodayInfoSiteMatchJob));
        await AddJobToSchedulerAsync(typeof(SyncFutureInfoSiteMatchJob));
        await AddJobToSchedulerAsync(typeof(SyncEventDeletedJob));
        await AddJobToSchedulerAsync(typeof(SyncInfositeIdJob));
        await AddJobToSchedulerAsync(typeof(Sync180DayInfositeMatchJob));

        //InfositeChildMatch
        await AddJobToSchedulerAsync(typeof(SyncTodayInfoSiteChildMatchJob));
        await AddJobToSchedulerAsync(typeof(SyncFutureInfoSiteChildMatchJob));
        await AddJobToSchedulerAsync(typeof(SyncDeletedChildInfoSiteMatchJob));

        //Report
        await AddJobToSchedulerAsync(typeof(SumTraderReportJob));
        await AddJobToSchedulerAsync(typeof(SumLaReportJob));
        await AddJobToSchedulerAsync(typeof(SumOvertimeReportJob));
        await AddJobToSchedulerAsync(typeof(SumPrematchReportJob));
        await AddJobToSchedulerAsync(typeof(SumMatchLiveReportJob));

        //OtherSport
        await AddJobToSchedulerAsync(typeof(SyncOtherSportAdminSiteMatchJob));

        //Others
        await AddJobToSchedulerAsync(typeof(SyncDataNotificationSettingJob));
        await AddJobToSchedulerAsync(typeof(MatchReadyNotificationJob));
        await AddJobToSchedulerAsync(typeof(SyncPreMatchLeagueJob));
        await AddJobToSchedulerAsync(typeof(SyncLeagueSettingsJob));
        await AddJobToSchedulerAsync(typeof(LinkLeagueAndTeamJob));
        await AddJobToSchedulerAsync(typeof(ArchiveOldMessageJob));
        await AddJobToSchedulerAsync(typeof(ArchiveDataJob));
        //Provider
        await AddJobToSchedulerAsync(typeof(SyncAgentConfirmJob));

        //Adminsite
        await AddJobToSchedulerAsync(typeof(SyncMatchInfoToAdminSiteJob));
        await AddJobToSchedulerAsync(typeof(SyncProviderMatchIdJob));

        // Chat
        await AddJobToSchedulerAsync(typeof(ChatJob));

        // Auto generate maxbet setting
        await AddJobToSchedulerAsync(typeof(AutoGenerateMaxbetSettingJob));

        // Push info to settlement
        await AddJobToSchedulerAsync(typeof(PushInfoToSettlementJob));
    }

    private async Task CreateScheduler()
    {
        Scheduler = await schedulerFactory.GetScheduler();
        Scheduler.JobFactory = jobFactory;
    }

    private async Task AddJobToSchedulerAsync(Type jobInfo)
    {
        var jobSetting = jobSettings.FirstOrDefault(job => job.Name.Equals(jobInfo.Name));
        if (jobSetting is { Enable: true })
        {
            var jobDetail = CreateJob(jobInfo);

            var trigger = CreateTrigger(jobInfo, jobSetting.CronExpression);

            await Scheduler.ScheduleJob(jobDetail, trigger);
        }
    }

    private async Task StartScheduler(CancellationToken cancellationToken)
    {
        await Scheduler.Start(cancellationToken);
    }

    private static ITrigger CreateTrigger(Type jobInfo, string cronExpression)
        => TriggerBuilder
            .Create()
            .WithIdentity($"trigger{jobInfo.Name}", $"group{jobInfo.Name}")
            .WithDescription(jobInfo.Name)
            .WithCronSchedule(cronExpression)
            .Build();

    private static IJobDetail CreateJob(Type jobInfo)
        => JobBuilder
            .Create(jobInfo)
            .WithIdentity($"sync{jobInfo.Name}", $"group{jobInfo.Name}")
            .Build();
}