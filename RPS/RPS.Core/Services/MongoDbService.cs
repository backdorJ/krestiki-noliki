using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using RPS.Domain.Entities;

namespace RPS.Core.Services;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["MongoDbSettings:ConnectionString"]);
        _database = client.GetDatabase(configuration["MongoDbSettings:DatabaseName"]);
    }

    public IMongoCollection<RatingMongo> Ratings => _database.GetCollection<RatingMongo>("RatingMongo");
}