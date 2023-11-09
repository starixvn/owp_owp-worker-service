namespace OWP.Infosite.Services;

using Constants;
using Extensions;
using Fanex.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using OWP.Services;
using Settings;

public interface ApiService
{
    Task<List<DeleteChildMatch>> QueryDeletedChildMatch(DateTime fromDate, DateTime toDate);

    Task<List<CommonMatchModel>> QueryAsync(ParamsQueryModel param);

    Task<List<MatchRelation>> QueryMatchesRelation(ParamsQueryModel param);

    Task<List<string>> GetInvalidEventKeysByDateAsync(DateTime date);

    Task<List<MatchModelQuery180Day>> QueryInfoSiteMatchFor180Day();

    Task<RoundTwInfo> QueryRound(DateTime dateFrom, DateTime dateTo);
}

public class ApiServiceImp : ApiService
{
    private readonly InfoSiteSettings infoSiteSettings;

    private readonly HttpHandler httpHandler;

    private readonly ILogger logger;

    public ApiServiceImp(HttpHandler httpHandler,
        ILogger logger,
        IOptions<InfoSiteSettings> infoSiteSettings)
    {
        this.httpHandler = httpHandler;
        this.logger = logger;
        this.infoSiteSettings = infoSiteSettings.Value;
    }

    public async Task<List<DeleteChildMatch>> QueryDeletedChildMatch(DateTime fromDate, DateTime toDate)
    {
        var dateFrom = fromDate.ToString(Constant.DateTimeFormat);
        var dateTo = toDate.ToString(Constant.DateTimeFormat);
        var requestLink = $"{infoSiteSettings.InfositeApi}api/EventSchedule/getInvalidChildEventInHouseByDate?date_from={dateFrom}&date_to={dateTo}";

        var response = await httpHandler.GetAsync(requestLink);
        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DeleteChildMatch>>(jsonData, new TrimmingConverter());
        }

        var logInfo = new
        {
            DateTime = DateTime.UtcNow,
            RequestUri = requestLink,
            StatusCode = (int)response.StatusCode,
            response.RequestMessage
        };
        var logInfoString = JsonConvert.SerializeObject(logInfo);
        await logger.ErrorAsync(logInfoString);

        return new List<DeleteChildMatch>();
    }

    public async Task<List<CommonMatchModel>> QueryAsync(ParamsQueryModel param)
    {
        var dateFrom = param.FromDate.ToString(Constant.DateTimeFormat);
        var dateTo = param.ToDate.ToString(Constant.DateTimeFormat);
        var stamp = param.LastUpdatedTime.ToString(Constant.DateTimeWithHourFormat);

        var requestLink = $"{infoSiteSettings.InfositeApi}api/EventSchedule/getEventsByDate?rd_checked=true&date_from={dateFrom}&date_to={dateTo}&stamp={stamp}";

        var response = await httpHandler.GetAsync(requestLink);
        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CommonMatchModel>>(jsonData, new TrimmingConverter());
        }

        var logInfo = new
        {
            DateTime = DateTime.UtcNow,
            RequestUri = requestLink,
            StatusCode = (int)response.StatusCode,
            response.RequestMessage
        };
        var logInfoString = JsonConvert.SerializeObject(logInfo);
        await logger.ErrorAsync(logInfoString);

        return new List<CommonMatchModel>();
    }

    public async Task<List<MatchRelation>> QueryMatchesRelation(ParamsQueryModel param)
    {
        var fromDate = param.FromDate.ToString(Constant.DateTimeFormat);
        var toDate = param.ToDate.ToString(Constant.DateTimeFormat);

        var requestLink = $"{infoSiteSettings.InfositeApi}api/EventSchedule/GetEventScheduleRelationForOWP?dateFrom={fromDate}&dateTo={toDate}";

        var response = await httpHandler.GetAsync(requestLink);

        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MatchRelation>>(jsonData, new TrimmingConverter());
        }

        var logInfo = new
        {
            DateTime = DateTime.UtcNow,
            RequestUri = requestLink,
            StatusCode = (int)response.StatusCode,
            response.RequestMessage
        };

        var logInfoString = JsonConvert.SerializeObject(logInfo);
        await logger.ErrorAsync(logInfoString);

        return new List<MatchRelation>();
    }

    public async Task<List<string>> GetInvalidEventKeysByDateAsync(DateTime date)
    {
        var dateInString = date.ToString(Constant.DateTimeFormat);

        var requestLink = $"{infoSiteSettings.InfositeApi}api/EventSchedule/getInvalidEventKeysByDate?rd_checked=true&date_from={dateInString}&date_to={dateInString}";

        var response = await httpHandler.GetAsync(requestLink);
        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<string>>(jsonData, new TrimmingConverter());
        }

        var logInfo = new
        {
            DateTime = DateTime.UtcNow,
            RequestPath = requestLink,
            StatusCode = (int)response.StatusCode,
            response.RequestMessage
        };
        var logInfoString = JsonConvert.SerializeObject(logInfo);
        await logger.ErrorAsync(logInfoString);

        return new List<string>();
    }

    public async Task<List<MatchModelQuery180Day>> QueryInfoSiteMatchFor180Day()
    {
        var requestLink = $"{infoSiteSettings.Infosite180DayApi}api/EventSchedule/GetEventScheduleForOWP?sport_id=1";
        var response = await httpHandler.GetAsync(requestLink);

        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MatchModelQuery180Day>>(jsonData, new TrimmingConverter());
        }

        var logInfo = new
        {
            DateTime = DateTime.UtcNow,
            RequestUri = requestLink,
            StatusCode = (int)response.StatusCode,
            response.RequestMessage
        };
        var logInfoString = JsonConvert.SerializeObject(logInfo);
        await logger.ErrorAsync(logInfoString);

        return new List<MatchModelQuery180Day>();
    }

    public async Task<RoundTwInfo> QueryRound(DateTime dateFrom, DateTime dateTo)
    {
        var requestLink = $"{infoSiteSettings.InfositeApi}api/OWP/getEventInfoByDate?date_from={dateFrom.ToString(Constant.DateTimeFormat)}&date_to={dateTo.ToString(Constant.DateTimeFormat)}";
        var response = await httpHandler.GetAsync(requestLink);

        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RoundTwInfo>(jsonData, new TrimmingConverter());
        }

        var logInfo = new
        {
            DateTime = DateTime.UtcNow,
            RequestUri = requestLink,
            StatusCode = (int)response.StatusCode,
            response.RequestMessage
        };
        var logInfoString = JsonConvert.SerializeObject(logInfo);
        await logger.ErrorAsync(logInfoString);

        return new ();
    }
}