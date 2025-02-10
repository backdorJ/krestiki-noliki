using TicTacToe.MediatR;

namespace RPS.Core.Requests.Game.JoinGame;

public class JoinGameCommand : IRequest
{
    public Guid GameId { get; set; }
}