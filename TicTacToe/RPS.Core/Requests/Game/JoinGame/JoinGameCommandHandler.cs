using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RPS.Core.Enums;
using RPS.Core.Hubs;
using RPS.Core.Interfaces;
using TicTacToe.Core.Interfaces;
using TicTacToe.MediatR;

namespace RPS.Core.Requests.Game.JoinGame;

public class JoinGameCommandHandler(IHubContext<GameHub> gameHubContext, IDbContext dbContext, IUserContext userContext)
    : IRequestHandler<JoinGameCommand>
{
    public async Task Handle(JoinGameCommand request, CancellationToken cancellationToken)
    {
        var game = await dbContext.Games
            .Include(x => x.Users)
            .FirstOrDefaultAsync(g => g.Id == request.GameId && !g.IsFinished, cancellationToken: cancellationToken);

        var currentUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken: cancellationToken);
        
        if (currentUser == null)
            throw new ArgumentNullException(nameof(currentUser));

        if (game.Users.Any(x => x.Id == currentUser.Id))
            return;

        await gameHubContext.Groups.AddToGroupAsync(
            userContext.UserId.ToString(),
            request.GameId.ToString(),
            cancellationToken);

        // Проверка рейтинга
        if (game?.Users.Count() < 2 && currentUser.Rating <= game.MaxRating)
        {
            game.Users.Add(currentUser);
            game.Status = GameStatus.Playing;
            await dbContext.SaveChangesAsync(cancellationToken);

            await gameHubContext.Clients.Group(request.GameId.ToString())
                .SendAsync("GameStarted", request.GameId.ToString(), cancellationToken: cancellationToken);
        }
        else
        {
            await  gameHubContext.Clients
                .Group(request.GameId.ToString())
                .SendAsync("JoinedAsSpectator", cancellationToken: cancellationToken);
        }
    }
}