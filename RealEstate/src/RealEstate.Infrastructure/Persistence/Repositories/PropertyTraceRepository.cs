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
    public sealed class PropertyTraceRepository : IPropertyTraceRepository
    {
        private readonly IMongoCollection<PropertyTrace> _collection;

        public PropertyTraceRepository(IMongoDbContext context)
        {
            _collection = context.PropertyTraces;
        }

        public async Task<PropertyTrace> CreateAsync(PropertyTrace entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<IReadOnlyList<PropertyTrace>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
        {
            var list = await _collection.Find(x => x.PropertyId == propertyId).ToListAsync(cancellationToken);
            return list;
        }
    }
}


