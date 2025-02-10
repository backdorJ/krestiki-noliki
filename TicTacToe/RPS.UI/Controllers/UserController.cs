using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPS.Core.Requests.User.GetUserRatings;
using TicTacToe.MediatR;

namespace RPS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpGet("get-rate-users")]
    public async Task<GetUsersRatingResponse> GetUsersRating(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserRatingsQuery(), cancellationToken);
        return result;
    }
}