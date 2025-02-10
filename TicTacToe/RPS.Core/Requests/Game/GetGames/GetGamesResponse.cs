namespace RPS.Core.Requests.Game.GetGames;

public class GetGamesResponse
{
    public List<GetGameResponseItem> Items { get; set; }
}

public class GetGameResponseItem
{
    public Guid GameId { get; set; }
    public string CreateUsername { get; set; }
    public string Status { get; set; }
    public string CreatedUserId { get; set; }
}