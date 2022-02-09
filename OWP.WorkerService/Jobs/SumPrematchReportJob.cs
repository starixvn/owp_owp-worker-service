using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.PrematchReport.Services.League;
using Quartz;

namespace OWP.WorkerService.Jobs
{
    public class SumPrematchReportJob : IJob
    {
        private readonly IMaintenanceService maintenanceService;

        private readonly LeagueService leagueService;

        public SumPrematchReportJob(IMaintenanceService maintenanceService, LeagueService leagueService)
        {
            this.maintenanceService = maintenanceService;
            this.leagueService = leagueService;
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
            var underMaintenanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();
            if (underMaintenanceTime.IsUnderMaintenanceTime)
            {
                return;
            }
            await LogService.InfoAsync($"Job {this.GetType()} is starting at {DateTime.Now}");
            await leagueService.BuildLeagueReport(DateTime.Today.AddDays(-1).Date).ConfigureAwait(false);
        }
    }
}
