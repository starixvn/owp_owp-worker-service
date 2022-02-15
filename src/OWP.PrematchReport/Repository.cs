using Fanex.Data.Repository;
using OWP.PrematchReport.Models;
using OWP.PrematchReport.Criteria;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace OWP.PrematchReport;

public interface Repository
{
  Task<IList<LeagueInfo>> GetLeagueByDateAndBettype(DateTime eventDate, int bettype);

  Task<IList<LeagueDetailInfo>> GetLeagueDetailByDate(DateTime eventDate, int bettype);

  Task InsertLeague(InsertLeagueDataReportCriteria criteria);

  Task InsertMatch(InsertMatchDataReportCriteria criteria);

  Task<IList<TotalDataMatch>> GetTotalMatchByLeagueIds(IList<int> PreMatchleagueIds, DateTime fromDate, DateTime toDate, DateTime frommDateNightShift);

  Task<TimeSpan?> QueryExistedStartTime(DateTime EventDate);
}

public class RepositoryImp : Repository
{
  private readonly IDynamicRepository repository;

  public RepositoryImp(IDynamicRepository repository)
  {
    this.repository = repository;
  }

  public async Task<IList<LeagueInfo>> GetLeagueByDateAndBettype(DateTime eventDate, int bettype)
  {
    var command = new GetLeagueCriteria()
    {
      EventDate = eventDate,
      Bettype = bettype
    };
    var leagueInfo = await repository.FetchAsync<LeagueInfo>(command);

    return leagueInfo.ToList();
  }

  public async Task<IList<LeagueDetailInfo>> GetLeagueDetailByDate(DateTime eventDate, int bettype)
  {
    var command = new GetLeagueDetailCriteria()
    {
      EventDate = eventDate,
      Bettype = bettype
    };
    var leagueInfo = await repository.FetchAsync<LeagueDetailInfo>(command);

    return leagueInfo.ToList();
  }

  public async Task<IList<TotalDataMatch>> GetTotalMatchByLeagueIds(IList<int> PreMatchleagueIds, DateTime fromDate, DateTime toDate, DateTime fromDateNightShift)
  {
    var command = new GetTotalMatchCriteria()
    {
      LeagueIds = String.Join(",", PreMatchleagueIds),
      FromDateTime = fromDate,
      ToDateTime = toDate,
      FromDateTimeNightShift = fromDateNightShift
    };
    var totals = await repository.FetchAsync<TotalDataMatch>(command);

    return totals.ToList();
  }

  public async Task<TimeSpan?> QueryExistedStartTime(DateTime EventDate)
  {
    var criteria = new GetStartTimeCriteria
    {
      EventDate = EventDate
    };
    var result = await repository.FetchAsync<TimeSpan>(criteria);

    return result.Any() ? new TimeSpan?(result.FirstOrDefault()) : null;
  }

  public async Task InsertLeague(InsertLeagueDataReportCriteria criteria)
    => await repository.ExecuteAsync(criteria);

  public async Task InsertMatch(InsertMatchDataReportCriteria criteria)
    => await repository.ExecuteAsync(criteria);
}