using TicTacToe.MediatR;

namespace TicTacToe.Core.Requests.Account.Register;

public class RegisterCommand : IRequest
{
    public RegisterCommand(string name, string password)
    {
        Password = password;
        Name = name;
    }

    public string Name { get; set; }
    
    public string Password { get; set; }
}