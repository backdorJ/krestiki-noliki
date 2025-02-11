using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RPS.Domain.Entities;

public class RatingMongo
{
    [BsonId] // Это ключевое поле, связанное с _id
    [BsonRepresentation(BsonType.ObjectId)] // Обеспечивает правильную конвертацию ObjectId
    public string Id { get; set; } 
    
    [BsonElement("UserId")]
    public string UserId { get; set; }

    [BsonElement("UserName")]
    public string UserName { get; set; }

    [BsonElement("Rating")]
    public int Rating { get; set; }
}