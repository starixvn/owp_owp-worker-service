using System.Text;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OWP.Adminsite.Models;
using OWP.Services;

namespace OWP.Adminsite.Services;

public interface IApiService
{
    Task<IEnumerable<MatchInfoApiModel>> PushMatchInfoAsync(DateTime fromDate, DateTime toDate, IEnumerable<MatchInfoApiModel> matchInfos);
}

public class ApiService(
    AdminSiteSettings apiSetting,
    HttpHandler httpHandler,
    IConfiguration configuration,
    ILogger logger) : IApiService
{
    public async Task<IEnumerable<MatchInfoApiModel>> PushMatchInfoAsync(DateTime fromDate, DateTime toDate, IEnumerable<MatchInfoApiModel> matchInfos)
    {
        var requestUri = apiSetting.PushMatchInfoAPI;
        var content = new StringContent(JsonConvert.SerializeObject(matchInfos), Encoding.UTF8, "application/json");

        var env = configuration.GetSection("Environment").Value;
        if (!string.Equals(env, "UAT") && !string.Equals(env, "PROD"))
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
}