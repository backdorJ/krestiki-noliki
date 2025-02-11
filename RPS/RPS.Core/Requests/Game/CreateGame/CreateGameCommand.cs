using TicTacToe.Core.Requests.Game.CreateGame;
using TicTacToe.MediatR;

namespace RPS.Core.Requests.Game.CreateGame;

public class CreateGameCommand : IRequest<CreateGameResponse>
{
    public CreateGameCommand(string roomName, uint maxRating)
    {
        RoomName = roomName;
        MaxRating = maxRating;
    }

    public string RoomName { get; set; }

    public uint MaxRating { get; set; }
}