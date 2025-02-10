using Microsoft.EntityFrameworkCore;
using RPS.Core.Interfaces;
using RPS.Domain.Entities;
using TicTacToe.Core.Interfaces;
using TicTacToe.Domain.Entities;

namespace TicTacToe.DAL;

public class EfContext : DbContext, IDbContext
{
    public EfContext(DbContextOptions<EfContext> optionsBuilder)
        : base(optionsBuilder)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfContext).Assembly); 
    }
}