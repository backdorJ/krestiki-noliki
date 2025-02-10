using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Core.Interfaces;

namespace TicTacToe.DAL;

public static class Entry
{
    public static void AddDal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<Migrator>();
        services.AddDbContext<IDbContext, EfContext>(options => options.UseNpgsql(configuration["PostgresConnectionString"]));
    }
}