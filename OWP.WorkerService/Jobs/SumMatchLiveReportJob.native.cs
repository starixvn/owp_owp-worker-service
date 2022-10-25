using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.SumReportData.Constants;
using OWP.SumReportData.Services;
using OWP.SumReportData.MatchLiveReport.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
public class SumMatchLiveReportJob : IJob
{
    private readonly ReportService service;
    private readonly CacheManipulator cacheManipulator;
    private readonly IMaintenanceService maintenanceService;

    public SumMatchLiveReportJob(CacheManipulator cacheManipulator, ReportService service, IMaintenanceService maintenanceService)
    {
        this.cacheManipulator = cacheManipulator;
        this.service = service;
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
        var underMaintenanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();
        if (underMaintenanceTime.IsUnderMaintenanceTime)
        {
            return;
        }

        var (isGenerateData, lastDate) = await cacheManipulator.GetLastDateForRegenerateData(Constant.MatchLiveLastDateCacheKey);
        var startDate = isGenerateData ? lastDate : DateTime.Today.AddDays(-3);

        for (var date = startDate; date < DateTime.Today; date = date.AddDays(1))
        {
            await service.Calculate(date, isGenerateData).ConfigureAwait(false);
        }

        await cacheManipulator.RemoveLastDateOfSumData(Constant.MatchLiveLastDateCacheKey);
    }
}