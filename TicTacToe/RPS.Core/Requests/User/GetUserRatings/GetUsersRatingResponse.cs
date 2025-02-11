namespace RPS.Core.Requests.User.GetUserRatings;

public class GetUsersRatingResponse
{
    public List<GetUsersRatingResponseItem> Users { get; set; }
}

public class GetUsersRatingResponseItem
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public int Rating { get; set; }
}