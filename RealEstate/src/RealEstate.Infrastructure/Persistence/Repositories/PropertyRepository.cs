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

            var list = await _collection
                .Find(filter)
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
                var regex = new BsonRegularExpression(text.Trim(), "i");
                filter &= builder.Or(
                    builder.Regex(x => x.Name, regex),
                    builder.Regex(x => x.Address.Street, regex),
                    builder.Regex(x => x.Address.City, regex)
                );
            }
            if (priceMin.HasValue)
            {
                filter &= builder.Gte(x => x.Price.Amount, priceMin.Value);
            }
            if (priceMax.HasValue)
            {
                filter &= builder.Lte(x => x.Price.Amount, priceMax.Value);
            }
            if (year.HasValue)
            {
                filter &= builder.Eq(x => x.Year, year.Value);
            }

            var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            var skip = (page - 1) * pageSize;
            var items = await _collection
                .Find(filter)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public async Task<bool> UpdateAsync(Property property, CancellationToken cancellationToken = default)
        {
            var result = await _collection.ReplaceOneAsync(x => x.Id == property.Id, property, cancellationToken: cancellationToken);
            return result.IsAcknowledged && result.ModifiedCount > 0;
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


