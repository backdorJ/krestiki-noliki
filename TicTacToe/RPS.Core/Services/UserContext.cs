using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TicTacToe.Core.Interfaces;

namespace TicTacToe.Core.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    Guid IUserContext.UserId =>
        Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)
            ? userId
            : Guid.Empty;
}