using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Persistence.Context;

namespace RealEstate.Infrastructure.Persistence.Repositories
{
    public sealed class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly IMongoCollection<PropertyImage> _collection;

        public PropertyImageRepository(IMongoDbContext context)
        {
            _collection = context.PropertyImages;
        }

        public async Task<PropertyImage> CreateAsync(PropertyImage entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<IReadOnlyList<PropertyImage>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
        {
            var list = await _collection.Find(x => x.PropertyId == propertyId).ToListAsync(cancellationToken);
            return list;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> SetEnabledAsync(Guid id, bool enabled, CancellationToken cancellationToken = default)
        {
            var update = Builders<PropertyImage>.Update.Set(x => x.Enabled, enabled);
            var result = await _collection.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}


