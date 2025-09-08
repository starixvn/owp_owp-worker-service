using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OWP.PushInfoToSettlement.Model;
using OWP.Services;
using System.Text;

namespace OWP.PushInfoToSettlement.Services
{
    public interface ISettlementApiService
    {
        Task<bool> ChangeInstantSettingAsync(IEnumerable<ChangeInstantSettingRequest> matchItems);
    }

    public class SettlementApiService(
        HttpHandler httpHandler, 
        SettlementApiSetting apiSetting,
        IConfiguration configuration,
        ILogger logger) : ISettlementApiService
    {
        public async Task<bool> ChangeInstantSettingAsync(IEnumerable<ChangeInstantSettingRequest> matchItems)
        {
            try
            {
                var requestUri = $"{apiSetting.Url}/api/match/change-instant-setting";
                var content = new StringContent(JsonConvert.SerializeObject(matchItems), Encoding.UTF8, "application/json");

                var env = configuration.GetSection("Environment").Value;
                if (string.Equals(env, "DEV") || string.Equals(env, "LOCAL"))
                {
                    var logInfo = new
                    {
                        Job = "Change Instant Setting to Settlement",
                        RequestUri = requestUri,
                        matchItems
                    };
                    await logger.InfoAsync(JsonConvert.SerializeObject(logInfo));

                    return true;
                }

                var response = await httpHandler.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var logInfo = new
                    {
                        Job = "Change Instant Setting to Settlement",
                        RequestUri = requestUri,
                        matchItems,
                        result = jsonData
                    };
                    await logger.InfoAsync(JsonConvert.SerializeObject(logInfo));

                    return true;
                }

                var logError = new
                {
                    Job = "Change Instant Setting to Settlement",
                    RequestUri = requestUri,
                    matchItems,
                    StatusCode = (int)response.StatusCode,
                    response.RequestMessage
                };
                await logger.ErrorAsync(JsonConvert.SerializeObject(logError));

                return false;
            }
            catch (Exception ex)
            {
                var logError = new
                {
                    Job = "Change Instant Setting to Settlement",
                    Error = "UnexpectedException",
                    Message = ex.Message,
                    matchItems,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                };
                await logger.ErrorAsync(JsonConvert.SerializeObject(logError));
                return false;
            }
        }
    }
}
