using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RPS.Core.Enums;
using RPS.Domain.Entities;
using TicTacToe.Core.Interfaces;

namespace RPS.Core.Hubs;

[Authorize]
public class GameHub : Hub
{
    private readonly IUserContext _userContext;
    private readonly IDbContext _dbContext;

    public GameHub(IUserContext userContext, IDbContext dbContext)
    {
        _userContext = userContext;
        _dbContext = dbContext;
    }

    // Присоединение к комнате
    public async Task JoinRoom(Guid gameId)
    {
        var game = await _dbContext.Games
            .Include(x => _dbContext.Users)
            .FirstOrDefaultAsync(g => g.Id == gameId && !g.IsFinished);

        var currentUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == _userContext.UserId);
        
        if (currentUser == null)
            throw new ArgumentNullException(nameof(currentUser));

        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());

        // Проверка рейтинга
        if (game?.Users.Count() < 2 && currentUser.Rating <= game.MaxRating)
        {
            game.Users.Add(currentUser);
            game.Status = GameStatus.Playing;
            await _dbContext.SaveChangesAsync();

            await Clients.Group(gameId.ToString()).SendAsync("GameStarted", gameId);
        }
        else
        {
            await Clients.Caller.SendAsync("JoinedAsSpectator");
        }
    }

    // Совершение хода
    public async Task MakeMove(Guid gameId, string move)
    {
        var game = await _dbContext.Games.Include(g => g.Moves).FirstOrDefaultAsync(g => g.Id == gameId);
        var currentUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == _userContext.UserId);

        if (currentUser == null || game == null || game.IsFinished)
            return;

        game.Moves.Add(new Move { UserId = currentUser.Id, Choice = move });
        await _dbContext.SaveChangesAsync();

        await Clients.Group(gameId.ToString()).SendAsync("MoveMade", new { Player = currentUser.Name, Move = move });

        if (game.Moves.Count == 2)
        {
            var result = DetermineWinner(game);
            await UpdateRatings(result, game);
            await SendResultToChat(result, game);

            game.IsFinished = true;
            await _dbContext.SaveChangesAsync();

            // Старт нового раунда через 5 секунд
            await Task.Delay(5000);
            await StartNewRound(game);
        }
    }

    private async Task StartNewRound(Game previousGame)
    {
        var newGame = new Game
        {
            Users = previousGame.Users,
            Status = GameStatus.Playing,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Games.Add(newGame);
        await _dbContext.SaveChangesAsync();

        await Clients.Group(previousGame.Id.ToString())
            .SendAsync("NewRoundStarted", newGame.Id);
    }

    private string DetermineWinner(Game game)
    {
        var move1 = game.Moves[0];
        var move2 = game.Moves[1];

        if (move1.Choice == move2.Choice)
            return "draw";

        return (move1.Choice, move2.Choice) switch
        {
            ("rock", "scissors") => move1.UserId.ToString(),
            ("scissors", "paper") => move1.UserId.ToString(),
            ("paper", "rock") => move1.UserId.ToString(),
            _ => move2.UserId.ToString()
        };
    }

    private async Task UpdateRatings(string winnerId, Game game)
    {
        if (winnerId == "draw")
            return;

        var winner = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == winnerId);
        var loser = game.Moves.First(m => m.UserId != winner?.Id).UserId;

        if (winner != null)
            winner.Rating += 3;

        var loserUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == loser);
        if (loserUser != null)
            loserUser.Rating -= 1;

        await _dbContext.SaveChangesAsync();
    }

    private async Task SendResultToChat(string result, Game game)
    {
        string message;
        if (result == "draw")
        {
            message = "Ничья!";
        }
        else
        {
            var winner = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == result);
            message = $"{winner?.Name} победил!";
        }

        await Clients.Group(game.Id.ToString())
            .SendAsync("GameResult", message);
    }
}
