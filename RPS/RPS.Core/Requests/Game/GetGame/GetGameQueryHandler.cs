using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using RPS.Core.Hubs;
using RPS.Core.Interfaces;
using RPS.Core.Services;
using TicTacToe.Core.Interfaces;
using TicTacToe.MediatR;

namespace RPS.Core.Requests.Game.GetGame;

public class GetGameQueryHandler(
        IDbContext dbContext, 
        GameHub hubContext, 
        IUserContext userContext,
        MongoDbService mongoDbService) 
    : IRequestHandler<GetGameQuery, GetGameResponse>
{

    public async Task<GetGameResponse> Handle(GetGameQuery request, CancellationToken cancellationToken)
    {
        var game = await dbContext.Games
            .Include(g => g.Users)
            .Include(g => g.Winner)
            .Include(g => g.Moves)
            .ThenInclude(g => g.User)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken: cancellationToken);

        if (game is null)
            throw new ArgumentException("Игра не найдена");

        // await _gameHub.JoinRoom(game.Id.ToString());
        
        var userRating = await mongoDbService.Ratings
            .Find(r => r.UserId == userContext.UserId.ToString())
            .FirstOrDefaultAsync();

        if (!game.IsFinished && (game.Users.Count < 2) && game.MaxRating < userRating.Rating)
            throw new ArgumentException("Не подходит рейтинг");
        
        var response = new GetGameResponse
        {
            CreateUsername = game.WhoCreatedName,
            GameId = game.Id,
            Status = game.Status.ToString(),
            WinnerName = game.Winner?.Name,
            Moves = game.Moves.Select(m => $"{m.User.Name}: {m.Choice}")
        };

        return response;
    }
}