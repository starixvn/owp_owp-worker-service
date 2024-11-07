namespace OWP.Infosite.Services;

using System.Text;
using System.Text.RegularExpressions;
using Constants;
using Extensions;
using Fanex.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OWP.Infosite.Models;
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

    Task<IsLiveSynchronizedResult> SetLiveMatches(IEnumerable<(int, int?)> liveMatches);

    Task<IsLiveSynchronizedResult> SetNonLiveMatches(IEnumerable<(int, int?)> nonLiveMatches);
}

public class ApiServiceImp : ApiService
{
    private static Regex regex = new Regex(@"^(\d+)\:.+$");
    private readonly InfoSiteSettings infoSiteSettings;
    private readonly HttpHandler httpHandler;
    private readonly ILogger logger;

    public ApiServiceImp(
        HttpHandler httpHandler,
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

    public async Task<IsLiveSynchronizedResult> SetLiveMatches(IEnumerable<(int, int?)> liveMatches)
    {
        return await SyncMatches(liveMatches, isLive: true);
    }

    public async Task<IsLiveSynchronizedResult> SetNonLiveMatches(IEnumerable<(int, int?)> nonLiveMatches)
    {
        return await SyncMatches(nonLiveMatches, isLive: false);
    }

    private async Task<IsLiveSynchronizedResult> SyncMatches(IEnumerable<(int, int?)> matches, bool isLive)
    {
        var request = $"{infoSiteSettings.InfositeApi}api/EventSchedule/setEventsLive";

        var model = GetKeyValuePairOfMatch(matches, isLive);
        var content = new StringContent(model, Encoding.UTF8, "application/json");

        var response = await httpHandler.PostAsync(request, content);
        var responseText = await response.Content.ReadAsStringAsync();

        var synchronizedResult = response.IsSuccessStatusCode ? responseText : "[]";
        var errorMessage = (response.IsSuccessStatusCode && responseText == "[]") ? string.Empty : responseText;

        var logInfo = new
        {
            Job = "Sync Is Live to Info Site",
            isLive,
            model,
            response.RequestMessage,
            StatusCode = (int)response.StatusCode,
            synchronizedResult
        };
        var logInfoString = JsonConvert.SerializeObject(logInfo);
        await logger.InfoAsync(logInfoString);

        var failedMatches = response.IsSuccessStatusCode ? GetFailedMatches(synchronizedResult) : matches.ToList();
        var successfulMatches = matches.Except(failedMatches).ToList();

        return new IsLiveSynchronizedResult
        {
            FailedMatches = failedMatches,
            SuccessfulMatches = successfulMatches,
            ErrorMessage = errorMessage
        };
    }

    private static string GetKeyValuePairOfMatch(IEnumerable<(int, int?)> matches, bool isLive)
    {
        var matchList = matches.GroupBy(x => x.Item1, x => x.Item2, (key, g) => (key, g.ToList()));
        var syncMatches = new List<LiveConfig>();

        foreach (var match in matchList)
        {
            (int mainMatchId, List<int?> subType) = match;

            if (subType.Exists(x => !x.HasValue))
            {
                syncMatches.Add(new LiveConfig
                {
                    MatchId = mainMatchId,
                    IsLive = isLive,
                    Subtypes = new List<int>()
                });
            }

            if (subType.Exists(x => x.HasValue))
            {
                syncMatches.Add(new LiveConfig
                {
                    MatchId = mainMatchId,
                    IsLive = isLive,
                    Subtypes = subType.Where(y => y.HasValue).Select(y => y.Value).ToList()
                });
            }
        }

        var result = new SyncIsLiveToInfoSite
        {
            Matches = syncMatches
        };
        return JsonConvert.SerializeObject(result);
    }

    private static List<(int, int?)> GetFailedMatches(string synchronizedResult)
    {
        var failedMessages = JsonConvert.DeserializeObject<List<string>>(synchronizedResult);
        var matches = failedMessages?.Where(l => regex.IsMatch(l))
            .Select(x => GetFailedMatch(x))
            .ToList();

        return matches ?? new List<(int, int?)>();
    }

    private static (int, int?) GetFailedMatch(string synchronizedResult)
    {
        var elements = synchronizedResult.Split(':');

        if (elements[1].Equals("FAILED_UPDATE_CHILD_MATCH"))
        {
            return (int.Parse(elements[0]), ParseSubtype(elements[2]));
        }

        return (int.Parse(elements[0]), null);
    }

    public static int? ParseSubtype(string result)
    {
        if (!string.IsNullOrEmpty(result))
        {
            var subtypeText = "subType ";
            var fromText = " from";
            var pFrom = result.IndexOf(subtypeText) + subtypeText.Length;
            var pTo = result.IndexOf(fromText);
            var subStringLength = pTo - pFrom;
            if (subStringLength < 0)
            {
                return null;
            }

            var subType = result.Substring(pFrom, pTo - pFrom);

            if (!string.IsNullOrWhiteSpace(subType))
            {
                return int.Parse(result.Substring(pFrom, pTo - pFrom));
            }
        }

        return null;
    }
}