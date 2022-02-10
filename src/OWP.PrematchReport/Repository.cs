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

  Task InsertLeague(InsertLeagueDataReportCriteria criteria);
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

  public async Task InsertLeague(InsertLeagueDataReportCriteria criteria)
      => await repository.ExecuteAsync(criteria);
}