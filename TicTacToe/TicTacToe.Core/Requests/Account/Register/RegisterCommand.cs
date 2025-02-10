using TicTacToe.MediatR;

namespace TicTacToe.Core.Requests.Account.Register;

public class RegisterCommand(string name, string password, string passwordConfirm) : IRequest
{
    public string Name { get; set; } = name;

    public string Password { get; set; } = password;

    public string PasswordConfirm { get; set; } = passwordConfirm;
}