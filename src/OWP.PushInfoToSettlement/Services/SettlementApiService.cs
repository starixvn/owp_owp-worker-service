using Fanex.Logging;
using Newtonsoft.Json;
using OWP.PushInfoToSettlement.Model;
using OWP.Services;
using System.Text;

namespace OWP.PushInfoToSettlement.Services
{
    public interface ISettlementApiService
    {
        Task<bool> ChangeInstantSettingAsync(IEnumerable<ChangeInstantSettingRequest> matchItems, DateTime fromDate, DateTime toDate);
    }

    public class SettlementApiService(
        HttpHandler httpHandler, 
        SettlementApiSetting apiSetting,
        ILogger logger) : ISettlementApiService
    {
        public async Task<bool> ChangeInstantSettingAsync(IEnumerable<ChangeInstantSettingRequest> matchItems, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var requestUri = $"{apiSetting.Url}/api/match/change-instant-setting";
                var requestBody = new { matchItems };
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await httpHandler.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var logInfo = new
                    {
                        Job = "Change Instant Setting to Settlement fromDate: " + fromDate + " toDate: " + toDate,
                        RequestUri = requestUri,
                        matchItems,
                        result = jsonData
                    };
                    await logger.InfoAsync(JsonConvert.SerializeObject(logInfo));

                    return true;
                }

                var logError = new
                {
                    Job = "Change Instant Setting to Settlement fromDate: " + fromDate + " toDate: " + toDate,
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
                    Job = "Change Instant Setting to Settlement fromDate: " + fromDate + " toDate: " + toDate,
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
