using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RealEstate.Application.DTOs.Property;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Domain.Entities;
using RealEstate.Domain.ValueObjects;
using RealEstate.Shared.Responses;

namespace RealEstate.Application.Services
{
    public interface IPropertyService
    {
        Task<PropertyResponseDto> CreateAsync(PropertyCreateDto dto, CancellationToken ct = default);
        Task<PropertyResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<PropertyResponseDto>> SearchAsync(Guid? ownerId, string? name, int skip, int take, CancellationToken ct = default);
        Task<PagedResponse<PropertyResponseDto>> SearchPagedAsync(PropertyFilterDto filter, CancellationToken ct = default);
        Task<bool> UpdateAsync(Guid id, PropertyUpdateDto dto, CancellationToken ct = default);
        Task<bool> UpdatePriceAsync(Guid id, PropertyPriceUpdateDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    }

    public sealed class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _repo;
        private readonly IMapper _mapper;

        public PropertyService(IPropertyRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PropertyResponseDto> CreateAsync(PropertyCreateDto dto, CancellationToken ct = default)
        {
            var address = new RealEstate.Domain.ValueObjects.Address(dto.Street, dto.City, dto.State, dto.Country, dto.ZipCode);
            var price = new Price(dto.Price, dto.Currency);
            var entity = new Property(dto.Name, address, price, dto.Year, dto.Area, dto.OwnerId, dto.Active);
            await _repo.CreateAsync(entity, ct);
            return _mapper.Map<PropertyResponseDto>(entity);
        }

        public async Task<PropertyResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            return entity is null ? null : _mapper.Map<PropertyResponseDto>(entity);
        }

        public async Task<IReadOnlyList<PropertyResponseDto>> SearchAsync(Guid? ownerId, string? name, int skip, int take, CancellationToken ct = default)
        {
            var list = await _repo.SearchAsync(ownerId, name, skip, take, ct);
            return _mapper.Map<IReadOnlyList<PropertyResponseDto>>(list);
        }

        public async Task<PagedResponse<PropertyResponseDto>> SearchPagedAsync(PropertyFilterDto filter, CancellationToken ct = default)
        {
            int page = filter.Page <= 0 ? 1 : filter.Page;
            int size = filter.PageSize <= 0 ? 20 : filter.PageSize;
            var (items, total) = await _repo.SearchPagedAsync(filter.OwnerId, filter.Text, filter.PriceMin, filter.PriceMax, filter.Year, page, size, ct);
            var dtoItems = _mapper.Map<IReadOnlyList<PropertyResponseDto>>(items);
            return new PagedResponse<PropertyResponseDto>
            {
                Page = page,
                PageSize = size,
                Total = total,
                Items = dtoItems
            };
        }

        public async Task<bool> UpdateAsync(Guid id, PropertyUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null) return false;
            var address = new RealEstate.Domain.ValueObjects.Address(dto.Street, dto.City, dto.State, dto.Country, dto.ZipCode);
            entity.UpdateBasics(dto.Name, address, dto.Year, dto.Area);
            return await _repo.UpdateAsync(entity, ct);
        }

        public Task<bool> UpdatePriceAsync(Guid id, PropertyPriceUpdateDto dto, CancellationToken ct = default)
        {
            return _repo.UpdatePriceAsync(id, new Price(dto.Amount, dto.Currency), ct);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            return _repo.DeleteAsync(id, ct);
        }
    }
}
