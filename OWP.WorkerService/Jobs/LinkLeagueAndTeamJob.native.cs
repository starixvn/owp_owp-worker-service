using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.LinkLeagueAndTeam.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
public class LinkLeagueAndTeamJob : IJob
{
    private readonly LinkLeagueAndTeamService linkLeagueAndTeamService;
    private readonly IMaintenanceService maintenanceService;

    public LinkLeagueAndTeamJob(LinkLeagueAndTeamService handler, IMaintenanceService maintenanceService)
    {
        this.linkLeagueAndTeamService = handler;
        this.maintenanceService = maintenanceService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await ExecuteJob();
        }
        catch (Exception e)
        {
            await LogService.ErrorAsync(e.Message, e);
        }
    }

    private async Task ExecuteJob()
    {
        await LogService.InfoAsync($"Job {this.GetType()} started at {DateTime.Now}");

        var underMaintenanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();
        if (underMaintenanceTime.IsUnderMaintenanceTime)
        {
            return;
        }

        await linkLeagueAndTeamService
            .Execute()
            .ConfigureAwait(false);
        await LogService.InfoAsync($"Job {this.GetType()} finished at {DateTime.Now}");
    }
}