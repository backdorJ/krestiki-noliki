using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RPS.Domain.Entities;
using TicTacToe.Core.Interfaces;

namespace RPS.Core.Hubs;

public class GameHub : Hub
{
    private readonly IUserContext _userContext;
    private readonly IDbContext _dbContext;

    public GameHub(IUserContext userContext, IDbContext dbContext)
    {
        _userContext = userContext;
        _dbContext = dbContext;
    }

    public async Task JoinGame(Guid gameId)
    {
        var game = await _dbContext.Games
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == gameId && !x.IsFinished);

        var currentUser = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == _userContext.UserId);
        
        if (currentUser == null)
            throw new ArgumentNullException(nameof(currentUser));

        // Присоединение к группе SignalR
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());

        if (game == null)
        {
            await Clients.Caller.SendAsync("Error", "Игра не найдена.");
            return;
        }

        if (game.Users.Count < 2 && !game.Users.Any(u => u.Id == currentUser.Id))
        {
            game.Users.Add(currentUser);
            await _dbContext.SaveChangesAsync();

            await Clients.Group(gameId.ToString())
                .SendAsync("PlayerJoined", currentUser.Name);
        }
        else
        {
            // Зритель
            await Clients.Caller.SendAsync("JoinedAsSpectator", "Вы подключены как зритель.");
        }
    }

    public async Task MakeMove(Guid gameId, string move)
    {
        var currentUser = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == _userContext.UserId);

        var game = await _dbContext.Games
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == gameId && !x.IsFinished);

        if (currentUser == null || game == null)
        {
            await Clients.Caller.SendAsync("Error", "Невозможно выполнить ход.");
            return;
        }

        if (!game.Users.Any(u => u.Id == currentUser.Id))
        {
            await Clients.Caller.SendAsync("Error", "Вы не являетесь игроком.");
            return;
        }

        // Логика обработки хода
        game.Moves.Add(new Move { UserId = currentUser.Id, Choice = move });
        await _dbContext.SaveChangesAsync();

        // Обновление состояния игры для всех (игроков и зрителей)
        await Clients.Group(gameId.ToString())
            .SendAsync("MoveMade", new { Player = currentUser.Name, Move = move });

        if (game.Moves.Count >= 2)
        {
            var result = DetermineWinner(game.Moves);
            game.IsFinished = true;
            await _dbContext.SaveChangesAsync();

            await Clients.Group(gameId.ToString())
                .SendAsync("GameFinished", result);
        }
    }

    private string DetermineWinner(List<Move> moves)
    {
        var move1 = moves[0];
        var move2 = moves[1];

        if (move1.Choice == move2.Choice)
            return "Ничья!";

        return (move1.Choice, move2.Choice) switch
        {
            ("rock", "scissors") => $"Победил игрок {move1.UserId}",
            ("scissors", "paper") => $"Победил игрок {move1.UserId}",
            ("paper", "rock") => $"Победил игрок {move1.UserId}",
            _ => $"Победил игрок {move2.UserId}"
        };
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == _userContext.UserId);

        if (user != null)
        {
            await Clients.All.SendAsync("PlayerLeft", user.Name);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
