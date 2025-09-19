using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using RealEstate.Domain.Entities;
using RealEstate.Domain.ValueObjects;
using RealEstate.Infrastructure.Persistence.Context;
using RealEstate.Application.Interfaces.Repositories;
using System.Text.RegularExpressions;

namespace RealEstate.Infrastructure.Persistence.Repositories
{
    public sealed class PropertyRepository : IPropertyRepository
    {
        private readonly IMongoCollection<Property> _collection;

        public PropertyRepository(IMongoDbContext context)
        {
            _collection = context.Properties;
        }

        public async Task<Property> CreateAsync(Property property, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(property, cancellationToken: cancellationToken);
            return property;
        }

        public async Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cursor = await _collection.FindAsync(x => x.Id == id, cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Property>> SearchAsync(Guid? ownerId = null, string? name = null, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
        {
            var builder = Builders<Property>.Filter;
            var filter = builder.Empty;
            if (ownerId.HasValue)
            {
                filter &= builder.Eq(x => x.OwnerId, ownerId.Value);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                filter &= builder.Regex(x => x.Name, new BsonRegularExpression(name.Trim(), "i"));
            }

            var projection = Builders<Property>.Projection
                .Include(x => x.Id)
                .Include(x => x.Name)
                .Include(x => x.Address.Street)
                .Include(x => x.Address.City)
                .Include(x => x.Address.State)
                .Include(x => x.Address.Country)
                .Include(x => x.Address.ZipCode)
                .Include(x => x.Price.Amount)
                .Include(x => x.Price.Currency)
                .Include(x => x.Year)
                .Include(x => x.Area)
                .Include(x => x.OwnerId)
                .Include(x => x.Active)
                .Include(x => x.CreatedAtUtc);

            var list = await _collection
                .Find(filter)
                .SortByDescending(x => x.CreatedAtUtc)
                .Project<Property>(projection)
                .Skip(skip)
                .Limit(take)
                .ToListAsync(cancellationToken);

            return list;
        }

        public async Task<(IReadOnlyList<Property> Items, long Total)> SearchPagedAsync(Guid? ownerId, string? text, decimal? priceMin, decimal? priceMax, int? year, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var builder = Builders<Property>.Filter;
            var filter = builder.Empty;
            if (ownerId.HasValue)
            {
                filter &= builder.Eq(x => x.OwnerId, ownerId.Value);
            }
            if (!string.IsNullOrWhiteSpace(text))
            {
                var escaped = Regex.Escape(text.Trim());
                var prefix = new BsonRegularExpression($"^{escaped}", "i");
                filter &= builder.Or(
                    builder.Regex(x => x.Name, prefix),
                    builder.Regex(x => x.Address.Street, prefix),
                    builder.Regex(x => x.Address.City, prefix),
                    builder.Regex(x => x.Address.State, prefix),
                    builder.Regex(x => x.Address.Country, prefix),
                    builder.Regex(x => x.Address.ZipCode, prefix)
                );
            }
            if (priceMin.HasValue || priceMax.HasValue)
            {
                // Usar $expr y $toDecimal para soportar datos almacenados como string o decimal
                if (priceMin.HasValue && priceMax.HasValue)
                {
                    var expr = new BsonDocument("$expr", new BsonDocument("$and", new BsonArray
                    {
                        new BsonDocument("$gte", new BsonArray { new BsonDocument("$toDecimal", "$Price.Amount"), priceMin.Value }),
                        new BsonDocument("$lte", new BsonArray { new BsonDocument("$toDecimal", "$Price.Amount"), priceMax.Value })
                    }));
                    filter &= expr;
                }
                else if (priceMin.HasValue)
                {
                    var expr = new BsonDocument("$expr", new BsonDocument("$gte", new BsonArray { new BsonDocument("$toDecimal", "$Price.Amount"), priceMin.Value }));
                    filter &= expr;
                }
                else if (priceMax.HasValue)
                {
                    var expr = new BsonDocument("$expr", new BsonDocument("$lte", new BsonArray { new BsonDocument("$toDecimal", "$Price.Amount"), priceMax.Value }));
                    filter &= expr;
                }
            }
            if (year.HasValue)
            {
                filter &= builder.Eq(x => x.Year, year.Value);
            }

            var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            var skip = (page - 1) * pageSize;

            var projection = Builders<Property>.Projection
                .Include(x => x.Id)
                .Include(x => x.Name)
                .Include(x => x.Address.Street)
                .Include(x => x.Address.City)
                .Include(x => x.Address.State)
                .Include(x => x.Address.Country)
                .Include(x => x.Address.ZipCode)
                .Include(x => x.Price.Amount)
                .Include(x => x.Price.Currency)
                .Include(x => x.Year)
                .Include(x => x.Area)
                .Include(x => x.OwnerId)
                .Include(x => x.Active)
                .Include(x => x.CreatedAtUtc);

            var items = await _collection
                .Find(filter)
                .SortByDescending(x => x.CreatedAtUtc)
                .Project<Property>(projection)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public async Task<bool> UpdateAsync(Property property, CancellationToken cancellationToken = default)
        {
            var result = await _collection.ReplaceOneAsync(x => x.Id == property.Id, property, cancellationToken: cancellationToken);
            // Consideramos éxito si hubo coincidencia aunque no se modificara ningún campo materializado (ModifiedCount puede ser 0)
            return result.IsAcknowledged && (result.ModifiedCount > 0 || result.MatchedCount > 0);
        }

        public async Task<bool> UpdatePriceAsync(Guid id, Price newPrice, CancellationToken cancellationToken = default)
        {
            var update = Builders<Property>.Update.Set(x => x.Price, newPrice);
            var result = await _collection.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}


