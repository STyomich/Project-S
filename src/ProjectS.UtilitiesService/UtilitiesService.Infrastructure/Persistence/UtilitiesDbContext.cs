
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace UtilitiesService.Infrastructure.Persistence;

public sealed class UtilitiesDbContext
{
    public IMongoDatabase Database { get; }

    public UtilitiesDbContext(IConfiguration config)
    {
        var client = new MongoClient(config["$MONGODB_CONNECTION_STRING"] ?? throw new ArgumentNullException("MongoDB connection string is not provided"));
        Database = client.GetDatabase(config["$MONGODB_DATABASE"] ?? throw new ArgumentNullException("MongoDB database name is not provided"));
    }
}
