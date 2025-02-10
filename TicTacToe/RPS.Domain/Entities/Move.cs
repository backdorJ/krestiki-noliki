using TicTacToe.Domain.Entities;

namespace RPS.Domain.Entities;

public class Move : BaseEntity
{
    public string Choice { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; }

    public Guid GameId { get; set; }
    
    public Game Game { get; set; }
}