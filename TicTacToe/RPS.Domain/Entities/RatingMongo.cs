using MongoDB.Bson.Serialization.Attributes;

namespace RPS.Domain.Entities;

public class RatingMongo
{
    [BsonElement("UserId")]
    public string UserId { get; set; }

    [BsonElement("UserName")]
    public string UserName { get; set; }

    [BsonElement("Rating")]
    public int Rating { get; set; }
}