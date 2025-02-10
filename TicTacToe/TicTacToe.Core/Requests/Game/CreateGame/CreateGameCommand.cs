using TicTacToe.MediatR;

namespace TicTacToe.Core.Requests.Game.CreateGame;

public class CreateGameCommand : IRequest<CreateGameResponse>
{
    public CreateGameCommand(string roomName)
    {
        RoomName = roomName;
    }

    public string RoomName { get; set; }
}