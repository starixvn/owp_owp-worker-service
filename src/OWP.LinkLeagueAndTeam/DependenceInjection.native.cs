using Microsoft.Extensions.DependencyInjection;
using OWP.LinkLeagueAndTeam.Services;

namespace OWP.LinkLeagueAndTeam;

public static class DependenceInjection
{
    public static IServiceCollection AddLinkLeagueAndTeamServices(this IServiceCollection services)
    {
        services.AddTransient<LinkLeagueAndTeamRepository, LinkLeagueAndTeamRepositoryImp>();
        services.AddTransient<LinkLeagueAndTeamService, LinkLeagueAndTeamServiceImp>();
        services.AddTransient<CacheManipulator, CacheManipulatorImp>();

        return services;
    }
}