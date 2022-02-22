using Fanex.Logging;
using ONEbook.UM.Services;
using Quartz;
using OWP.PrematchReport.Services;
namespace OWP.WorkerService.Jobs
{
  public class SumPrematchReportJob : IJob
  {
    private readonly IMaintenanceService maintenanceService;

    private readonly PrematchReportService reportService;

    public SumPrematchReportJob(
      IMaintenanceService maintenanceService,
      PrematchReportService reportService)
    {
      this.maintenanceService = maintenanceService;
      this.reportService = reportService;
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
      await reportService.CalculateDataReport().ConfigureAwait(false);
    }
  }
}
