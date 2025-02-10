using Microsoft.EntityFrameworkCore;
using TicTacToe.Core.Interfaces;
using TicTacToe.MediatR;

namespace RPS.Core.Requests.Game.GetGames;

public class GetGamesQueryHandler : IRequestHandler<GetGamesQuery, GetGamesResponse>
{
    private readonly IDbContext _dbContext;

    public GetGamesQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetGamesResponse> Handle(GetGamesQuery request, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Games
            .Where(x => !x.IsFinished)
            .Select(x => new GetGameResponseItem
            {
                GameId = x.Id,
                CreateUsername = x.WhoCreatedName,
                Status = x.Status.ToString(),
                CreatedUserId = null
            })
            .ToListAsync(cancellationToken);

        return new GetGamesResponse()
        {
            Items = result,
        };
    }
}