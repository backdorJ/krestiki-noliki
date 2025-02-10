using RPS.Core.Enums;

namespace TicTacToe.Domain.Entities;

public class Game : BaseEntity
{
    public string RoomName { get; set; }
    public Guid WinnerId { get; set; }
    public string WhoCreatedName { get; set; }
    public bool IsFinished { get; set; } = false;

    public GameStatus Status { get; set; }

    public List<User> Users { get; set; } = new List<User>();
}