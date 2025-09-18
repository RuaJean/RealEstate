using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Persistence.Context;
using RealEstate.Application.Interfaces.Repositories;

namespace RealEstate.Infrastructure.Persistence.Repositories
{
    public sealed class OwnerRepository : IOwnerRepository
    {
        private readonly IMongoCollection<Owner> _collection;

        public OwnerRepository(IMongoDbContext context)
        {
            _collection = context.Owners;
        }

        public async Task<Owner> CreateAsync(Owner owner, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(owner, cancellationToken: cancellationToken);
            return owner;
        }

        public async Task<Owner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cursor = await _collection.FindAsync(x => x.Id == id, cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Owner>> SearchAsync(string? name = null, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
        {
            var filter = string.IsNullOrWhiteSpace(name)
                ? Builders<Owner>.Filter.Empty
                : Builders<Owner>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(name.Trim(), "i"));

            var cursor = await _collection
                .Find(filter)
                .Skip(skip)
                .Limit(take)
                .ToListAsync(cancellationToken);

            return cursor;
        }

        public async Task<bool> UpdateAsync(Owner owner, CancellationToken cancellationToken = default)
        {
            var result = await _collection.ReplaceOneAsync(x => x.Id == owner.Id, owner, cancellationToken: cancellationToken);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}


