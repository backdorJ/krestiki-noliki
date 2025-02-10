using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicTacToe.Domain.Entities;

namespace TicTacToe.DAL.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IsFinished);
        builder.Property(x => x.RoomName).HasDefaultValue("TicTacToe Room");
        builder.Property(x => x.WinnerId);

        builder.HasMany(x => x.Users)
            .WithMany(x => x.Games);
    }
}