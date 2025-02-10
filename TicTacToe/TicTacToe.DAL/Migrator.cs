using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TicTacToe.DAL;

public class Migrator
{
    private readonly EfContext _context;
    private readonly ILogger<Migrator> _logger;
    
    public Migrator(EfContext context, ILogger<Migrator> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task MigrateAsync()
    {
        try
        {
            _logger.LogInformation("Migrating database...");

            await _context.Database.MigrateAsync();
            
            _logger.LogInformation("Migrating database...");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogCritical("An error occurred while migrating the database.");
            throw;
        }
    }
}