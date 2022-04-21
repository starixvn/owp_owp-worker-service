using Microsoft.Extensions.DependencyInjection;
using OWP.OtherSport.SyncAdminSiteId.Handlers;
using OWP.OtherSport.SyncLive;
using OWP.OtherSport.SyncLive.Services;

namespace OWP.OtherSport
{
    public static class DependenceInjection
    {
        public static IServiceCollection AddOtherSportServices(this IServiceCollection services)
        {
            services.AddTransient<SyncAdminSiteMatchesHandler, SyncAdminSiteMatchesHandlerImp>();
            services.AddTransient<SyncLiveToAdminSiteHandler, SyncLiveToAdminSiteHandlerImp>();
            services.AddTransient<EditEventGenerator, EditEventGeneratorImp>();
            services.AddTransient<ChangedSyncLiveDetector, ChangedSyncLiveDetectorImp>();
            services.AddTransient<ApiService, ApiServiceImp>();
            return services;
        }
    }
}