using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using Newtonsoft.Json;
using OWP.Infosite.Constants;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;
using System.Globalization;

namespace OWP.Infosite.Services;

public interface SyncPreMatchLeagueService
{
  Task Sync();
}

public class SyncPreMatchLeagueServiceImp : SyncPreMatchLeagueService
{
  private readonly DateTimeService dateTimeService;

  private readonly IDynamicRepository dynamicRepository;

  private readonly ICache cacheService;

  private readonly ApiService apiService;

  private readonly ILogger logger;

  public SyncPreMatchLeagueServiceImp(
    DateTimeService dateTimeService,
    IDynamicRepository dynamicRepository,
    ICache cacheService,
     ApiService apiService,
     ILogger logger)
  {
    this.dateTimeService = dateTimeService;
    this.dynamicRepository = dynamicRepository;
    this.cacheService = cacheService;
    this.apiService = apiService;
    this.logger = logger;
  }

  public async Task Sync()
  {
    var sessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
    var preMatchLeagues = await GetOldPreMatchLeagues(dateTimeService.Today);
    await SaveOldPreMatchLeaguesSnapshot(sessionId, preMatchLeagues);

    var criteria = new SyncInfoSiteMatchToPreMatchLeagueCriteria
    {
      FromDate = dateTimeService.Today,
      ToDate = dateTimeService.Today.AddYears(10)
    };
    await dynamicRepository.ExecuteAsync(criteria);

    apiService.TriggerNetCoreNewLeagueNotification(
          sessionId,
            DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified)
       );
  }

  private async Task<IEnumerable<PreMatchLeague>> GetOldPreMatchLeagues(DateTime fromDate)
  {
    var criteria = new GetPreMatchLeaguesCriteria
    {
      FromDate = fromDate
    };

    return await dynamicRepository.FetchAsync<PreMatchLeague>(criteria);
  }

  private async Task SaveOldPreMatchLeaguesSnapshot(string sessionId, IEnumerable<PreMatchLeague> preMatchLeagues)
  {
    var expiredDate = DateTime.Today.AddDays(Constant.CacheExpiredInDays);
    var absoluteTimeSpan = expiredDate - DateTime.Now;
    var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(absoluteTimeSpan);
    var cacheKey = $"{Constant.OldPreMatchLeaguesSnapshotKey}-{sessionId}";
    var leaguesJson = JsonConvert.SerializeObject(preMatchLeagues);
    await cacheService.SetAsync(cacheKey, leaguesJson, cachePolicy);
  }
}