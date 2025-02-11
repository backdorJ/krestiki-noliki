using Microsoft.EntityFrameworkCore;
using RPS.Core.Hubs;
using RPS.Core.Interfaces;
using TicTacToe.MediatR;

namespace RPS.Core.Requests.Game.GetGame;

public class GetGameQueryHandler(IDbContext dbContext, GameHub hubContext) 
    : IRequestHandler<GetGameQuery, GetGameResponse>
{
    private readonly GameHub _gameHub = hubContext;

    public async Task<GetGameResponse> Handle(GetGameQuery request, CancellationToken cancellationToken)
    {
        var game = await dbContext.Games
            .Include(g => g.Moves)
            .ThenInclude(g => g.User)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken: cancellationToken);

        if (game is null)
            throw new ArgumentException("Игра не найдена");

        // await _gameHub.JoinRoom(game.Id.ToString());
        
        var response = new GetGameResponse
        {
            CreateUsername = game.WhoCreatedName,
            GameId = game.Id,
            Status = game.Status.ToString(),
            Moves = game.Moves.Select(m => $"The Player {m.User.Name} made a move - {m.Choice}")
        };

        return response;
    }
}