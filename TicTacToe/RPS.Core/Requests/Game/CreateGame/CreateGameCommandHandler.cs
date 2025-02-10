using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RPS.Core.Enums;
using RPS.Core.Hubs;
using RPS.Core.Interfaces;
using RPS.Core.Requests.Game.GetGames;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Requests.Game.CreateGame;
using TicTacToe.MediatR;

namespace RPS.Core.Requests.Game.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, CreateGameResponse>
{
    private readonly IDbContext _dbContext;
    private readonly IUserContext _userContext; 
    private readonly IHubContext<GameHub> _hubContext;
    
    public CreateGameCommandHandler(IDbContext dbContext, IUserContext userContext, IHubContext<GameHub> hubContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _hubContext = hubContext;
    }

    public async Task<CreateGameResponse> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        
        var currentUser = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == _userContext.UserId, cancellationToken)
            ?? throw new ApplicationException($"User with id {_userContext.UserId} not found");

        var game = new RPS.Domain.Entities.Game
        {
            RoomName = request.RoomName,
            WhoCreatedName = currentUser.Name,
            Status = GameStatus.Waiting,
            MaxRating =  request.MaxRating,
            Users = new List<TicTacToe.Domain.Entities.User>()
            {
                currentUser
            }
        };
        
        await _dbContext.Games.AddAsync(game, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _hubContext.Clients.All.SendAsync("GetCreatedGameNotify", new GetGameResponseItem
        {
            GameId = game.Id,
            CreateUsername = currentUser.Name,
            Status = GameStatus.Waiting.ToString(),
            CreatedUserId = currentUser.Id.ToString(),
        }, cancellationToken);
        
        return new CreateGameResponse
        {
            Id = game.Id
        };
    }
}