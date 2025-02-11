using Microsoft.Extensions.DependencyInjection;
using RPS.Core.Services;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Services;

namespace RPS.Core;

public static class Entry
{
    public static void AddCore(this IServiceCollection services)
    {
        services.AddScoped<IJwtGenerator, JwtGenerator>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<MongoDbService>();
    }
}