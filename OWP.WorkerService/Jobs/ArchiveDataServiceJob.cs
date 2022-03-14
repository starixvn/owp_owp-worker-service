using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.ArchiveData.Services;
using Quartz;

namespace OWP.WorkerService.Jobs
{
    public class ArchiveDataServiceJob : IJob
    {
        private readonly IMaintenanceService maintenanceService;

        private readonly ArchiveDataService archiveDataService;

        public ArchiveDataServiceJob(
          IMaintenanceService maintenanceService,
          ArchiveDataService archiveDataService)
        {
            this.maintenanceService = maintenanceService;
            this.archiveDataService = archiveDataService;
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
            await archiveDataService.DeleteOldMessage(DateTime.Now).ConfigureAwait(false);
        }
    }
}