using Fanex.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OWP.Core.Services;
using OWP.Infosite.Settings;
using OWP.OtherSport.SyncLive.Model;
using System.Text;

namespace OWP.OtherSport.SyncLive.Services
{
    public interface ApiService
    {
        Task PostSyncLiveAsync(BodyModel body);
    }

    public class ApiServiceImp : ApiService
    {
        private readonly InfoSiteSettings infoSiteSettings;

        private readonly HttpHandler httpHandler;

        private readonly ILogger logger;

        public ApiServiceImp(IOptions<InfoSiteSettings> infoSiteSettings, ILogger logger, HttpHandler httpHandler)
        {
            this.infoSiteSettings = infoSiteSettings.Value;
            this.httpHandler = httpHandler;
            this.logger = logger;
        }

        public async Task PostSyncLiveAsync(BodyModel body)
        {
            var path = "api/EventSchedule/SetEventLiveByInHouseMatchId";
            var infositeDomain = infoSiteSettings.InfositeOSAPI;
            if (string.IsNullOrEmpty(infositeDomain)) return; 
            
            var requestUrl = $"{infositeDomain}{path}";
            var json = JsonConvert.SerializeObject(body);
            var bodyContent = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpHandler.PostAsync(requestUrl, bodyContent);
                var responseText = await response.Content.ReadAsStringAsync();
                await LogInfo(requestUrl, body, responseText);
            }
            catch (Exception ex)
            {
                await LogError(requestUrl, body, ex);
            }
        }

        private async Task LogError(string requestUrl, BodyModel body, Exception ex)
        {
            var logErrorString = JsonConvert.SerializeObject(new
            {
                DateTime = DateTime.UtcNow.AddHours(8),
                RequestUri = requestUrl,
                BodyContent = body,
                ex.Message
            });

            await logger.ErrorAsync($"OSSyncLiveToInfosite {logErrorString}");
        }

        private async Task LogInfo(string requestUrl, BodyModel body, string reponse)
        {
            var logInfoString = JsonConvert.SerializeObject(new
            {
                DateTime = DateTime.UtcNow.AddHours(8),
                RequestUri = requestUrl,
                BodyContent = body,
                Response = reponse
            });

            await logger.InfoAsync($"OSSyncLiveToInfosite {logInfoString}");
        }
    }
}
