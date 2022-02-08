using Microsoft.Extensions.DependencyInjection;
using OWP.PrematchReport;

namespace OWP.PrematchReport
{
    public static class DependenceInjection
    {
        public static IServiceCollection AddPrematchReportServices(this IServiceCollection services)
        {
            services.AddTransient<Repository, RepositoryImp>();

            return services;
        }
    }
}