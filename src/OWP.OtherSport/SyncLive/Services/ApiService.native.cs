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

    public class ApiServiceImp: ApiService
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
            var path = "/api/Event/Edit";
            var requestUrl = $"{infoSiteSettings.InfositeAPI}{path}";

            var json = JsonConvert.SerializeObject(body);
            var bodyContent = new StringContent(json, Encoding.UTF8, "application/json");

            await LogInfo(requestUrl, body);
            try
            {
                await httpHandler.PostAsync(requestUrl, bodyContent);
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

            await logger.ErrorAsync(logErrorString);
        }

        private async Task LogInfo(string requestUrl, BodyModel body)
        {
            var logInfoString = JsonConvert.SerializeObject(new
            {
                DateTime = DateTime.UtcNow.AddHours(8),
                RequestUri = requestUrl,
                BodyContent = body
            });

            await logger.InfoAsync($"OSSyncLiveToInfosite {logInfoString}");
        }
    }
}
