using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPS.Core.Requests.Account.Register;
using TicTacToe.Core.Requests.Account.Login;
using TicTacToe.Core.Requests.Account.Register;
using TicTacToe.MediatR;
using LoginRequest = TicTacToe.Core.Requests.Account.Login.LoginRequest;
using RegisterRequest = TicTacToe.Core.Requests.Account.Register.RegisterRequest;

namespace TicTacToe.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Post(RegisterRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await mediator.Send(new RegisterCommand(request.Name, request.Password, request.PasswordConfirm), cancellationToken);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var result = await mediator.Send(new LoginCommand(request.Name, request.Password), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public string TEst()
    {
        return "asdasdad";
    }
}