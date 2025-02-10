using Microsoft.EntityFrameworkCore;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfContext).Assembly); 
    }
}