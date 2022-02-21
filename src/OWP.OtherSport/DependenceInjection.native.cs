using Microsoft.Extensions.DependencyInjection;
using OWP.OtherSport.SyncAdminSiteId.Handlers;

namespace OWP.OtherSport
{
    public static class DependenceInjection
    {
        public static IServiceCollection AddOtherSportServices(this IServiceCollection services)
        {
            services.AddTransient<SyncAdminSiteMatchesHandler, SyncAdminSiteMatchesHandlerImp>();
            return services;
        }
    }
}