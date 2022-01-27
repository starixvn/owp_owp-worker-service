using Fanex.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OWP.Core.Extensions;
using OWP.Infosite.Constants;
using OWP.Infosite.Models;
using OWP.Infosite.Settings;

namespace OWP.Infosite.Services;

public interface ApiService
{
    Task<List<CommonMatchModel>> QueryAsync(ParamsQueryModel param);

    Task TriggerNetCoreMatchInfoNotification(string sessionId, DateTime fromDate, DateTime toDate);

    Task<List<MatchRelation>> QueryMatchesRelation(ParamsQueryModel param);

    Task TriggerNetCoreNewLeagueNotification(string sessionId, DateTime fromDate);

    Task<List<string>> GetInvalidEventKeysByDateAsync(DateTime date);

    Task<List<MatchModelQuery180Day>> QueryInfoSiteMatchFor180Day();
}

public class ApiServiceImp : ApiService
{
    private readonly InfoSiteSettings infoSiteSettings;
    private readonly HttpHandler httpHandler;
    private readonly ILogger logger;

    public ApiServiceImp(HttpHandler httpHandler, ILogger logger, IOptions<InfoSiteSettings> infoSiteSettings)
    {
        this.httpHandler = httpHandler;
        this.logger = logger;
        this.infoSiteSettings = infoSiteSettings.Value;
    }

    public async Task<List<CommonMatchModel>> QueryAsync(ParamsQueryModel param)
    {
        var date_from = param.FromDate.ToString(Constant.DateTimeFormat);
        var date_to = param.ToDate.ToString(Constant.DateTimeFormat);
        var stamp = param.LastUpdatedTime.ToString(Constant.DateTimeWithHourFormat);

        var requestLink = $"{infoSiteSettings.InfositeAPI}api/EventSchedule/getEventsByDate?rd_checked=true&date_from={date_from}&date_to={date_to}&stamp={stamp}";

        var response = await httpHandler.GetAsync(requestLink);
        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CommonMatchModel>>(jsonData, new TrimmingConverter());
        }
        else
        {
            var logInfo = new
            {
                DateTime = DateTime.UtcNow,
                RequestUri = requestLink,
                StatusCode = (int)response.StatusCode,
                response.RequestMessage
            };
            var logInfoString = JsonConvert.SerializeObject(logInfo);
            logger.Error(logInfoString);

            return new List<CommonMatchModel>();
        }
    }

    public async Task<List<MatchRelation>> QueryMatchesRelation(ParamsQueryModel param)
    {
        var fromDate = param.FromDate.ToString(Constant.DateTimeFormat);
        var toDate = param.ToDate.ToString(Constant.DateTimeFormat);

        var requestLink = $"{infoSiteSettings.InfositeAPI}api/EventSchedule/GetEventScheduleRelationForOWP?dateFrom={fromDate}&dateTo={toDate}";

        var response = await httpHandler.GetAsync(requestLink);

        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MatchRelation>>(jsonData, new TrimmingConverter());
        }
        else
        {
            var logInfo = new
            {
                DateTime = DateTime.UtcNow,
                RequestUri = requestLink,
                StatusCode = (int)response.StatusCode,
                response.RequestMessage
            };

            var logInfoString = JsonConvert.SerializeObject(logInfo);
            logger.Error(logInfoString);

            return new List<MatchRelation>();
        }
    }

    public async Task TriggerNetCoreMatchInfoNotification(string sessionId, DateTime fromDate, DateTime toDate)
    {
        var requestLink = $"{infoSiteSettings.OWPNetCoreUri}/PreMatchNotification/notify-match-info?sessionId={sessionId}&fromDate={fromDate}&toDate={toDate}";
        await httpHandler.GetAsync(requestLink);
    }

    public async Task TriggerNetCoreNewLeagueNotification(string sessionId, DateTime fromDate)
    {
        var requestLink = $"{infoSiteSettings.OWPNetCoreUri}/PreMatchNotification/notify-new-league?sessionId={sessionId}&fromDate={fromDate}";

        await httpHandler.GetAsync(requestLink);
    }

    public async Task<List<string>> GetInvalidEventKeysByDateAsync(DateTime date)
    {
        var dateInString = date.ToString(Constant.DateTimeFormat);

        var requestLink = $"{infoSiteSettings.InfositeAPI}api/EventSchedule/getInvalidEventKeysByDate?rd_checked=true&date_from={dateInString}&date_to={dateInString}";

        var response = await httpHandler.GetAsync(requestLink);
        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<string>>(jsonData, new TrimmingConverter());
        }
        else
        {
            var logInfo = new
            {
                DateTime = DateTime.UtcNow,
                RequestPath = requestLink,
                StatusCode = (int)response.StatusCode,
                response.RequestMessage
            };
            var logInfoString = JsonConvert.SerializeObject(logInfo);
            logger.Error(logInfoString);

            return new List<string>();
        }
    }

    public async Task<List<MatchModelQuery180Day>> QueryInfoSiteMatchFor180Day()
    {
        var requestLink = $"{infoSiteSettings.Infosite180DayAPI}api/EventSchedule/GetEventScheduleForOWP?sport_id=1";
        var response = await httpHandler.GetAsync(requestLink);

        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MatchModelQuery180Day>>(jsonData, new TrimmingConverter());
        }
        else
        {
            var logInfo = new
            {
                DateTime = DateTime.UtcNow,
                RequestUri = requestLink,
                StatusCode = (int)response.StatusCode,
                response.RequestMessage
            };
            var logInfoString = JsonConvert.SerializeObject(logInfo);
            logger.Error(logInfoString);

            return new List<MatchModelQuery180Day>();
        }
    }
}