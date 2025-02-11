using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using RPS.Core.Enums;
using RPS.Core.Interfaces;
using RPS.Core.Services;
using RPS.Domain.Entities;
using RPS.Shared.Models;
using TicTacToe.Core.Interfaces;

namespace RPS.Core.Hubs;

[Authorize]
public class GameHub : Hub
{
    private readonly IUserContext _userContext;
    private readonly IDbContext _dbContext;
    private readonly MongoDbService _mongoDbService;
    private readonly IPublishEndpoint _publishEndpoint; 

    public GameHub(IUserContext userContext, IDbContext dbContext, MongoDbService mongoDbService, IPublishEndpoint publishEndpoint)
    {
        _userContext = userContext;
        _dbContext = dbContext;
        _mongoDbService = mongoDbService;
        _publishEndpoint = publishEndpoint;
    }

    // Присоединение к комнате
    public async Task JoinRoom(string gameId)
    {
        var game = await _dbContext.Games
            .Include(x => x.Users)
            .FirstOrDefaultAsync(g => g.Id == Guid.Parse(gameId));

        var currentUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == _userContext.UserId);

        if (currentUser == null)
            throw new ArgumentNullException(nameof(currentUser));

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            gameId);

        if (game.Users.Any(x => x.Id == currentUser.Id))
        {
            await Clients.Group(gameId)
                .SendAsync("JoinedGameInfo", new
                {
                    IsPlayer = true,
                    game.Status
                });

            return;
        }
        
        RatingMongo rating = null;
        
        var userRating = await _mongoDbService.Ratings
            .Find(r => r.UserId == currentUser.Id.ToString())
            .FirstOrDefaultAsync();
        
        if (userRating == null)
        {
            await _publishEndpoint.Publish<RatingMessage>(new
            {
                UserId = currentUser.Id.ToString(),
                UserName = currentUser.Name,
                Rating = 0,
            });
        }
        
        var currentRating = rating?.Rating ?? userRating?.Rating ?? 0;

        // Проверка рейтинга
        if (game?.Users.Count() < 2 && currentRating <= game.MaxRating)
        {
            game.Users.Add(currentUser);
            game.Status = GameStatus.Playing;
            await _dbContext.SaveChangesAsync();

            await Clients.Group(gameId)
                .SendAsync(
                    "JoinedGameInfo",
                    new
                    {
                        IsPlayer = true,
                        game.Status
                    });
        }
    }

    // Совершение хода
    public async Task MakeMove(string gameId, string move)
    {
        var game = await _dbContext.Games
            .Include(g => g.Moves)
            .Include(g => g.Users) // Подключаем пользователей, чтобы их можно было исключить
            .FirstOrDefaultAsync(g => g.Id == Guid.Parse(gameId));

        var currentUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == _userContext.UserId);

        if (currentUser == null || game == null || game.IsFinished)
            return;

        // Сохраняем ход
        game.Moves.Add(new Move { UserId = currentUser.Id, Choice = move });
        await _dbContext.SaveChangesAsync();

        // Получаем список всех connectionId игроков в этой игре
        var playerConnectionIds = game.Users
            .Where(u => u.Id != currentUser.Id) // Исключаем текущего игрока, если нужно
            .Select(u => u.HubConnection) // Предполагается, что у пользователя есть ConnectionId
            .ToList();

        // Отправляем сообщение всем, кроме игроков
        await Clients.GroupExcept(gameId, playerConnectionIds)
            .SendAsync("MoveMade", new { Message = $"The Player {currentUser.Name} made a move - {move}" });

        // Логика определения победителя
        if (game.Moves.Count == 2)
        {
            var result = DetermineWinner(game);
            await UpdateRatings(result, game);
            await SendResultToChat(result, game);

            game.IsFinished = true;
            game.Status = GameStatus.Finished;
            await _dbContext.SaveChangesAsync();

            // Старт нового раунда через 5 секунд
            await Task.Delay(5000);
            await StartNewRound(game);
        }
    }

    public override async Task OnConnectedAsync()
    {
        var currentUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == _userContext.UserId);
        if (currentUser != null)
        {
            currentUser.HubConnection = Context.ConnectionId ?? string.Empty;
            await _dbContext.SaveChangesAsync();
        }

        await base.OnConnectedAsync();
    }

    private async Task StartNewRound(Game previousGame)
    {
        var newGame = new Game
        {
            Users = previousGame.Users,
            Status = GameStatus.Playing,
            CreatedAt = DateTime.UtcNow,
            WhoCreatedName = previousGame.WhoCreatedName,
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

        // Получение данных из EF (только для идентификации пользователей)
        
        var winner = game.Users.FirstOrDefault(x => x.Id.ToString() == winnerId);
        var loserUser = game.Users.FirstOrDefault(x => x.Id.ToString() != winnerId);

        RatingMongo winnerRating = null;
        RatingMongo loserRating = null;
        
        if (winner != null)
        {
            winnerRating = await _mongoDbService.Ratings
                .Find(r => r.UserId == winner.Id.ToString())
                .FirstOrDefaultAsync();

            if (winnerRating == null)
            {   
                await _publishEndpoint.Publish<RatingMessage>(new
                {
                    UserId = winner.Id.ToString(),
                    UserName = winner.Name,
                    Rating = 3,
                });
            }
            else
            {
                // Обновляем рейтинг
                winnerRating.Rating += 3;
                await _mongoDbService.Ratings.ReplaceOneAsync(
                    r => r.UserId == winner.Id.ToString(),
                    winnerRating);
            }
        }

        if (loserUser != null)
        {
            loserRating = await _mongoDbService.Ratings
                .Find(r => r.UserId == loserUser.Id.ToString())
                .FirstOrDefaultAsync();

            if (loserRating == null)
            {
                await _publishEndpoint.Publish<RatingMessage>(new
                {
                    UserId = loserUser.Id.ToString(),
                    UserName = loserUser.Name,
                    Rating = -1
                });
            }
            else
            {
                loserRating.Rating -= 1;
                await _mongoDbService.Ratings.ReplaceOneAsync(
                    r => r.UserId == loserUser.Id.ToString(),
                    loserRating);
            }
        }
        
        // Отправка результата в группу
        await Clients.Group(game.Id.ToString()).SendAsync(
            "GameResult",
            new
            {
                Message = $"Rating player: {winner?.Name} - {winnerRating?.Rating}\n" +
                          $"Rating player: {loserUser?.Name} - {loserRating?.Rating}"
            });
    }

    private async Task SendResultToChat(string result, Game game)
    {
        string winnerMessage;
        if (result == "draw")
        {
            winnerMessage = "Ничья!";
        }
        else
        {
            var winner = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == result);
            winnerMessage = $"{winner?.Name} победил!";
        }

        await Clients
            .Group(game.Id.ToString())
            .SendAsync("GameResult", new { Message = winnerMessage });
    }
}