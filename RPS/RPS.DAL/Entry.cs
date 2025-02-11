using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RPS.Core.Interfaces;
using RPS.Core.Services;
using TicTacToe.DAL;

namespace RPS.DAL;

public static class Entry
{
    public static void AddDal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<Migrator>();
        services.AddSingleton<MongoDbService>();
        services.AddDbContext<IDbContext, EfContext>(options => options.UseNpgsql(configuration["PostgresConnectionString"]));
    }
}