using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicTacToe.Domain.Entities;

namespace RPS.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Password).IsRequired();
        builder.Property(x => x.Rating).HasDefaultValue(0);
        
        builder.HasMany(x => x.Moves)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id);
    }
}