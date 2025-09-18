using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces.Repositories
{
    public interface IOwnerRepository
    {
        Task<Owner> CreateAsync(Owner owner, CancellationToken cancellationToken = default);
        Task<Owner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Owner>> SearchAsync(string? name = null, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Owner owner, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
