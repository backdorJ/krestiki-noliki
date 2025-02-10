using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RPS.Domain.Entities;

namespace RPS.DAL.Configurations;

public class MoveConfiguration : IEntityTypeConfiguration<Move>
{
    public void Configure(EntityTypeBuilder<Move> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Choice);
        builder.HasOne(x => x.Game)
            .WithMany(x => x.Moves)
            .HasForeignKey(x => x.GameId)
            .HasPrincipalKey(x => x.Id);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Moves)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id);
    }
}