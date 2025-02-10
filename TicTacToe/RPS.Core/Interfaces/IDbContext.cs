using Microsoft.EntityFrameworkCore;
using TicTacToe.Domain.Entities;

namespace TicTacToe.Core.Interfaces;

public interface IDbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Game> Games { get; set; }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}