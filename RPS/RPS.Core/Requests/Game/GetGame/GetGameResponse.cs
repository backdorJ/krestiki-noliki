namespace RPS.Core.Requests.Game.GetGame;

public class GetGameResponse
{
    public Guid GameId { get; set; }
    public string CreateUsername { get; set; }
    public string Status { get; set; }
    public string? WinnerName { get; set; }
    public IEnumerable<string> Moves { get; set; }
}