using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RealEstate.Application.DTOs.PropertyImage;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services
{
    public interface IPropertyImageService
    {
        Task<PropertyImageResponseDto> CreateAsync(PropertyImageCreateDto dto, CancellationToken ct = default);
        Task<IReadOnlyList<PropertyImageResponseDto>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default);
        Task<bool> SetEnabledAsync(Guid id, bool enabled, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    }

    public sealed class PropertyImageService : IPropertyImageService
    {
        private readonly IPropertyImageRepository _repo;
        private readonly IMapper _mapper;

        public PropertyImageService(IPropertyImageRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PropertyImageResponseDto> CreateAsync(PropertyImageCreateDto dto, CancellationToken ct = default)
        {
            var entity = new PropertyImage(dto.PropertyId, dto.Url, dto.Description, dto.Enabled);
            await _repo.CreateAsync(entity, ct);
            return _mapper.Map<PropertyImageResponseDto>(entity);
        }

        public async Task<IReadOnlyList<PropertyImageResponseDto>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default)
        {
            var list = await _repo.GetByPropertyIdAsync(propertyId, ct);
            return _mapper.Map<IReadOnlyList<PropertyImageResponseDto>>(list);
        }

        public Task<bool> SetEnabledAsync(Guid id, bool enabled, CancellationToken ct = default)
        {
            return _repo.SetEnabledAsync(id, enabled, ct);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            return _repo.DeleteAsync(id, ct);
        }
    }
}
