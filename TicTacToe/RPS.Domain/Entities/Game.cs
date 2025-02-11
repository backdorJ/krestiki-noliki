using RPS.Core.Enums;
using TicTacToe.Domain.Entities;

namespace RPS.Domain.Entities;

public class Game : BaseEntity
{
    public string RoomName { get; set; }
    public Guid WinnerId { get; set; }
    public string WhoCreatedName { get; set; }
    public bool IsFinished { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    
    public uint MaxRating { get; set; }

    public GameStatus Status { get; set; }

    public List<Move> Moves { get; set; } = new List<Move>();

    public List<User> Users { get; set; } = new List<User>();
}