using TicTacToe.MediatR;

namespace RPS.Core.Requests.Account.Register;

public class RegisterCommand : IRequest
{
    public RegisterCommand(string name, string password, string passwordConfirm)
    {
        Password = password;
        Name = name;
        PasswordConfirm = passwordConfirm;
    }

    public string Name { get; set; }
    
    public string Password { get; set; }

    public string PasswordConfirm { get; set; }
}