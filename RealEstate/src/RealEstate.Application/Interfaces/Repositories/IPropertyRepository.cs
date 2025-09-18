using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;
using RealEstate.Domain.ValueObjects;

namespace RealEstate.Application.Interfaces.Repositories
{
    public interface IPropertyRepository
    {
        Task<Property> CreateAsync(Property entity, CancellationToken cancellationToken = default);
        Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Property>> SearchAsync(Guid? ownerId = null, string? name = null, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Property> Items, long Total)> SearchPagedAsync(Guid? ownerId, string? text, decimal? priceMin, decimal? priceMax, int? year, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Property entity, CancellationToken cancellationToken = default);
        Task<bool> UpdatePriceAsync(Guid id, Price newPrice, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
