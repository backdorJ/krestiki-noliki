namespace TicTacToe.Core.Interfaces;

public interface IJwtGenerator
{
    public string GenerateJwtToken(string username, Guid userId);
}