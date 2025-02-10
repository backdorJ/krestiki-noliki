using TicTacToe.MediatR;

namespace TicTacToe.Core.Requests.Account.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    public LoginCommand(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; set; }
    
    public string Password { get; set; }
}