using MongoDB.Driver;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Context
{
    public sealed class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
    }

    public interface IMongoDbContext
    {
        IMongoCollection<Owner> Owners { get; }
        IMongoCollection<Property> Properties { get; }
        IMongoCollection<PropertyImage> PropertyImages { get; }
        IMongoCollection<PropertyTrace> PropertyTraces { get; }
        IMongoCollection<User> Users { get; }
        IMongoDatabase Database { get; }
    }

    public sealed class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoClient _client;
        public IMongoDatabase Database { get; }

        public IMongoCollection<Owner> Owners => Database.GetCollection<Owner>("owners");
        public IMongoCollection<Property> Properties => Database.GetCollection<Property>("properties");
        public IMongoCollection<PropertyImage> PropertyImages => Database.GetCollection<PropertyImage>("property_images");
        public IMongoCollection<PropertyTrace> PropertyTraces => Database.GetCollection<PropertyTrace>("property_traces");
        public IMongoCollection<User> Users => Database.GetCollection<User>("users");

        public MongoDbContext(MongoDbSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            Database = _client.GetDatabase(settings.Database);
        }
    }
}


