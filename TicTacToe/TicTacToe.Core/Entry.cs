using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Services;

namespace TicTacToe.Core;

public static class Entry
{
    public static void AddCore(this IServiceCollection services)
    {
        services.AddScoped<IJwtGenerator, JwtGenerator>();
        services.AddScoped<IUserContext, UserContext>();
    }
}