using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OWP.Core.Extensions;
using OWP.Core.Services;
using OWP.Infosite.Models;
using OWP.Infosite.Services;
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
        private readonly IConfiguration configuration;

        private readonly HttpHandler httpHandler;

        private readonly ILogger logger;

        public ApiServiceImp(IConfiguration configuration, ILogger logger, HttpHandler httpHandler)
        {
            this.configuration = configuration;
            this.httpHandler = httpHandler;
            this.logger = logger;
        }

        public async Task PostSyncLiveAsync(BodyModel body)
{
            var url = configuration.GetSection("AdminSite:Url").Value;
            var requestLink = $"{url}/api/Event/Edit";

            var json = JsonConvert.SerializeObject(body);
            var bodyContent = new StringContent(json, Encoding.UTF8, "application/json");

            await logger.InfoAsync($"OSSyncLiveToAdminSite {requestLink} {json}");
            try
            {
                await httpHandler.PostAsync(requestLink, bodyContent);
            }
            catch (Exception ex)
            {
                var logInfo = new
                {
                    DateTime = DateTime.UtcNow,
                    RequestUri = requestLink,
                    ex.Message
                };
                var logInfoString = JsonConvert.SerializeObject(logInfo);
                //logger.Error(logInfoString);
            }
        }
    }
}
