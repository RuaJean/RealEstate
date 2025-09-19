using MongoDB.Driver;
using MongoDB.Bson;
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
        private readonly MongoClient _client;
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
            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            // Índice de texto para búsqueda por múltiples campos en propiedades
            var propertyCollection = Properties;
            var textIndexKeys = Builders<Property>.IndexKeys
                .Text(p => p.Name)
                .Text(p => p.Address.Street)
                .Text(p => p.Address.City)
                .Text(p => p.Address.State)
                .Text(p => p.Address.Country)
                .Text(p => p.Address.ZipCode);

            var textIndexOptions = new CreateIndexOptions
            {
                Name = "properties_text_search",
                Weights = new BsonDocument
                {
                    { nameof(Property.Name), 10 },
                    { "Address.City", 5 },
                    { "Address.Street", 3 },
                    { "Address.State", 2 },
                    { "Address.Country", 2 },
                    { "Address.ZipCode", 1 }
                },
                Background = true
            };

            try
            {
                propertyCollection.Indexes.CreateOne(new CreateIndexModel<Property>(textIndexKeys, textIndexOptions));
                // Índices para acelerar búsquedas por prefijo (regex ^) en campos usados
                var nameIndex = new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(p => p.Name), new CreateIndexOptions { Name = "idx_property_name" });
                var cityIndex = new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(p => p.Address.City), new CreateIndexOptions { Name = "idx_property_city" });
                var streetIndex = new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(p => p.Address.Street), new CreateIndexOptions { Name = "idx_property_street" });
                var stateIndex = new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(p => p.Address.State), new CreateIndexOptions { Name = "idx_property_state" });
                var countryIndex = new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(p => p.Address.Country), new CreateIndexOptions { Name = "idx_property_country" });
                var zipIndex = new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(p => p.Address.ZipCode), new CreateIndexOptions { Name = "idx_property_zip" });
                var ownerIndex = new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(p => p.OwnerId), new CreateIndexOptions { Name = "idx_property_owner" });
                var priceIndex = new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(p => p.Price.Amount), new CreateIndexOptions { Name = "idx_property_price" });
                var yearIndex = new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(p => p.Year), new CreateIndexOptions { Name = "idx_property_year" });
                propertyCollection.Indexes.CreateMany(new[] { nameIndex, cityIndex, streetIndex, stateIndex, countryIndex, zipIndex, ownerIndex, priceIndex, yearIndex });
            }
            catch
            {
                // Ignorar si no se puede crear (p.ej. permisos) para no bloquear el arranque.
                // La API seguirá pudiendo usar filtros no-texto; $text requerirá crear el índice manualmente.
            }
        }
    }
}


