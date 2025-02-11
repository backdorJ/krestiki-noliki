namespace RPS.Core.Requests.Game.CreateGame;

public class CreateGameRequest
{
    public string RoomName { get; set; }

    public uint MaxRating { get; set; }
}