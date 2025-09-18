using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces.Repositories
{
    public interface IPropertyTraceRepository
    {
        Task<PropertyTrace> CreateAsync(PropertyTrace entity, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PropertyTrace>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    }
}
