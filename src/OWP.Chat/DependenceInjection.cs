using OWP.Chat.Service;

namespace OWP.Chat;

using Microsoft.Extensions.DependencyInjection;

public static class DependenceInjection
{
    public static IServiceCollection AddChatServices(this IServiceCollection services)
    {
        services.AddTransient<Repository, RepositoryImp>();
        services.AddTransient<ChatService, ChatServiceImp>();
        return services;
    }
}