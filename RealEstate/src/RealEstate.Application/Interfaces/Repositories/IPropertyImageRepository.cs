using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces.Repositories
{
    public interface IPropertyImageRepository
    {
        Task<PropertyImage> CreateAsync(PropertyImage entity, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PropertyImage>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> SetEnabledAsync(Guid id, bool enabled, CancellationToken cancellationToken = default);
    }
}
