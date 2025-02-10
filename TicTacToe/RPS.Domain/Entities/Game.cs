namespace TicTacToe.Domain.Entities;

public class Game : BaseEntity
{
    public string RoomName { get; set; }
    public Guid WinnerId { get; set; }
    public bool IsFinished { get; set; } = false;

    public List<User> Users { get; set; } = new List<User>();
}