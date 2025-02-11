using MassTransit;
using RPS.Core.Services;
using RPS.Domain.Entities;
using RPS.Shared.Models;

namespace RPS.Core.Consumers;

public class GameRatingConsumer(MongoDbService mongoDbService) : IConsumer<RatingMessage>
{
    public async Task Consume(ConsumeContext<RatingMessage> context)
    {
        var message = context.Message;
        var rating = new RatingMongo
        {
            UserId = message.UserId.ToString(),
            UserName = message.Username,
            Rating = message.Rating
        };
        
        await mongoDbService.Ratings.InsertOneAsync(rating);
    }
}