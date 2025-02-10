using Microsoft.EntityFrameworkCore;
using RPS.Core.Interfaces;
using TicTacToe.MediatR;

namespace RPS.Core.Requests.User.GetUserRatings;

public class GetUserRatingsQueryHandler(IDbContext dbContext)
    : IRequestHandler<GetUserRatingsQuery, GetUsersRatingResponse>
{
    public async Task<GetUsersRatingResponse> Handle(GetUserRatingsQuery request, CancellationToken cancellationToken)
    {
        var result = await dbContext.Users
            .OrderByDescending(x => x.Rating)
            .Select(x => new GetUsersRatingResponseItem
            {
                UserId = x.Id,
                Username = x.Name,
                Rating = x.Rating
            })
            .ToListAsync(cancellationToken);

        return new GetUsersRatingResponse
        {
            Users = result,
        };
    }
}