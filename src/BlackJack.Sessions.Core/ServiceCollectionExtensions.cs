using BlackJack.Sessions.Core.Abstractions.Repositories;
using BlackJack.Sessions.Core.Abstractions.Services;
using BlackJack.Sessions.Core.Repositories;
using BlackJack.Sessions.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlackJack.Sessions.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlackJackSessions(this IServiceCollection services)
    {
        services.AddTransient<IBlackJackSessionsService, BlackJackSessionsService>();
        services.AddTransient<IBlackJackSessionsRepository, BlackJackSessionsRepository>();
        return services;
    }
}