namespace TicTacToe.Core.Requests.Account.Register;

public record class RegisterRequest(string Name, string Password, string PasswordConfirm);