using Microsoft.Extensions.DependencyInjection;
using OWP.PrematchReport.Services;
using OWP.PrematchReport.Services.League;
using OWP.PrematchReport.Services.Match;

namespace OWP.PrematchReport
{
  public static class DependenceInjection
  {
    public static IServiceCollection AddPrematchReportServices(this IServiceCollection services)
    {
      services.AddTransient<Repository, RepositoryImp>();
      services.AddTransient<DataReportService, DataReportBuilderServiceImp>();
      services.AddTransient<DataMatchReportService, DataMatchReportServiceImp>();
      services.AddTransient<LeagueService, LeagueServiceImp>();
      services.AddTransient<OULeagueService, OULeagueServiceImp>();
      services.AddTransient<HDPLeagueService, HDPLeagueServiceImp>();
      services.AddTransient<MatchService, MatchServiceImp>();
      services.AddTransient<OUMatchService, OUMatchServiceImp>();
      services.AddTransient<HDPMatchService, HDPMatchServiceImp>();
      services.AddTransient<DataReportService, DataReportBuilderServiceImp>();
      services.AddTransient<DbWriterService, DbWriterServiceImp>();
      services.AddTransient<PrematchReportService, PrematchReportServiceImp>();
      services.AddTransient<CacheManipulator, CacheManipulatorImp>();
      return services;
    }
  }
}