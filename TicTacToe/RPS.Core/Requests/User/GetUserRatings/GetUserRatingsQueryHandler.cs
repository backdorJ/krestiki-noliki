using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using RPS.Core.Interfaces;
using RPS.Core.Services;
using TicTacToe.MediatR;

namespace RPS.Core.Requests.User.GetUserRatings;

public class GetUserRatingsQueryHandler(MongoDbService mongoDbService)
    : IRequestHandler<GetUserRatingsQuery, GetUsersRatingResponse>
{
    public async Task<GetUsersRatingResponse> Handle(GetUserRatingsQuery request, CancellationToken cancellationToken)
    {
        // Используем Find и Sort для MongoDB
        var result = await mongoDbService.Ratings
            .Find(_ => true) // Получение всех записей
            .SortByDescending(x => x.Rating) // Сортировка по рейтингу
            .Project(x => new GetUsersRatingResponseItem
            {
                UserId = x.UserId,
                Username = x.UserName,
                Rating = x.Rating
            })
            .ToListAsync(cancellationToken);

        return new GetUsersRatingResponse
        {
            Users = result,
        };
    }
}