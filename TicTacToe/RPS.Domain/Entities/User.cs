using TicTacToe.Domain.Entities;

namespace RPS.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string HubConnection { get; set; } = string.Empty;

    public List<Game> Games { get; set; } = new List<Game>();

    public List<Move> Moves { get; set; } = new List<Move>();
}