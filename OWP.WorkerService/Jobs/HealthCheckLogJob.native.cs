namespace OWP.WorkerService.Jobs;

using System.Diagnostics;
using Quartz;

[DisallowConcurrentExecution]
public class HealthCheckLogJob : IJob
{
    private static HttpClient sharedClient = new();

    private string environment;

    public HealthCheckLogJob(IConfiguration configuration)
    {
        environment = configuration.GetSection("Environment").Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        while (environment == "PROD")
        {
            await HealthCheck("nap_settlement_PROD");
            await HealthCheck("nap_tvs_website_PROD");
            await Task.Delay(1000);
        }

        await Task.CompletedTask;
    }

    private static async Task HealthCheck(string service)
    {
        var watch = Stopwatch.StartNew();
        var url = $"http://{service}:5000/healthcheck";
        try
        {
            using var response = await sharedClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var text = await response.Content.ReadAsStringAsync();                
                watch.Stop();
                await WriteLog(service, $"Health check {service} response: {text}", watch.ElapsedMilliseconds);
            }
            else
            {
                watch.Stop();
                await WriteLog(service, $"Health check {service} error status code: {response.StatusCode}", watch.ElapsedMilliseconds);
            }
        }
        catch (Exception e)
        {
            watch.Stop();
            await WriteLog(service, $"Health check {service} error: {e}", watch.ElapsedMilliseconds);
        }
    }

    private static async Task WriteLog(string service, string text, long elapsedMilliseconds)
    {
        var logFileSuffix = elapsedMilliseconds >= 3000 ? "slow" : "fast";
        var logFile = $"/app/App_Data/{service}-logs-{logFileSuffix}-{DateTime.Today:yyyyMMdd}.txt";
        var line = $"{DateTime.Now}\t{text}\t{elapsedMilliseconds} ms\r\n";
        
        await File.AppendAllTextAsync(logFile, line);
    }
}