using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OWP.Adminsite.Models;
using OWP.Extensions;
using OWP.Services;
using System.Text;

namespace OWP.Adminsite.Services;

public interface IApiService
{
    Task<ProviderMatchIdTw> QueryProviderMatchId(DateTime date);
    Task<IEnumerable<MatchInfoApiModel>> PushMatchInfoAsync(DateTime fromDate, DateTime toDate, IEnumerable<MatchInfoApiModel> matchInfos);
}

public class ApiService(
    AdminSiteSettings apiSetting,
    HttpHandler httpHandler,
    IConfiguration configuration,
    LogMonitoringService logMonitoringService,
    ILogger logger) : IApiService
{
    public const string DateTimeFormat = "yyyy-MM-dd";

    public async Task<IEnumerable<MatchInfoApiModel>> PushMatchInfoAsync(DateTime fromDate, DateTime toDate, IEnumerable<MatchInfoApiModel> matchInfos)
    {
        var requestUri = apiSetting.PushMatchInfoAPI;
        var content = new StringContent(JsonConvert.SerializeObject(matchInfos), Encoding.UTF8, "application/json");

        var env = configuration.GetSection("Environment").Value;
        if (string.Equals(env, "DEV") || string.Equals(env, "LOCAL"))
        {
            var logInfo = new
            {
                Job = "Sync match info to AdminSite",
                fromDate,
                toDate,
                RequestUri = requestUri,
                matchInfos
            };
            await logger.InfoAsync(JsonConvert.SerializeObject(logInfo));

            return [];
        }

        var response = await httpHandler.PostAsync(requestUri, content);

        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            var logInfo = new
            {
                Job = "Sync match info to AdminSite",
                fromDate,
                toDate,
                RequestUri = requestUri,
                matchInfos,
                result = jsonData
            };
            await logger.InfoAsync(JsonConvert.SerializeObject(logInfo));

            return [];
        }

        var logError = new
        {
            Job = "Sync match info to AdminSite",
            fromDate,
            toDate,
            RequestUri = requestUri,
            matchInfos,
            StatusCode = (int)response.StatusCode,
            response.RequestMessage
        };
        await logger.ErrorAsync(JsonConvert.SerializeObject(logError));

        return matchInfos;
    }

    public async Task<ProviderMatchIdTw> QueryProviderMatchId(DateTime date)
    {
        var requestLink = $"{apiSetting.InfositeAPI}api/OWP/GetSportsTickerMapping?sportType=1&date={date.ToString(DateTimeFormat)}";
        var response = await httpHandler.GetAsync(requestLink);
        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            await logMonitoringService.LogDetailedPayload(date, "QueryProviderMatchId", jsonData);
            return JsonConvert.DeserializeObject<ProviderMatchIdTw>(jsonData, new TrimmingConverter());
        }

        var logInfo = new
        {
            DateTime = date,
            RequestUri = requestLink,
            StatusCode = (int)response.StatusCode,
            response.RequestMessage,
            Action = "QueryProviderMatchId"
        };
        var logInfoString = JsonConvert.SerializeObject(logInfo);
        await logger.ErrorAsync(logInfoString);

        return new ProviderMatchIdTw();
    }
}