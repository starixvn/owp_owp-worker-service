namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using OWP.AutoGenerateMaxbetSetting.Services;
using Quartz;

[DisallowConcurrentExecution]
public class AutoGenerateMaxbetSettingJob(GeneratorService generatorService) : IJob
{
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
        await generatorService.GenerateMaxbetSetting().ConfigureAwait(false);

    }
}