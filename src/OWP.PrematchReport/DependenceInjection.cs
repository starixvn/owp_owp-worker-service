using Microsoft.Extensions.DependencyInjection;
using OWP.PrematchReport.Services;
using OWP.PrematchReport.Services.League;

namespace OWP.PrematchReport
{
    public static class DependenceInjection
    {
        public static IServiceCollection AddPrematchReportServices(this IServiceCollection services)
        {
            services.AddTransient<Repository, RepositoryImp>();
            services.AddTransient<DataReportBuilderService, DataReportBuilderServiceImp>();
            services.AddTransient<LeagueService, LeagueServiceImp>();
            services.AddTransient<OULeagueService, OULeagueServiceImp>();
            services.AddTransient<DataReportBuilderService, DataReportBuilderServiceImp>();
            services.AddTransient<DbWriterService, DbWriterServiceImp>();

            return services;
        }
    }
}