namespace TicTacToe.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;
}