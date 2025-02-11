using TicTacToe.MediatR;

namespace RPS.Core.Requests.Game.GetGame;

public class GetGameQuery(Guid id) : IRequest<GetGameResponse>
{
    public Guid Id { get; set; } = id;
}