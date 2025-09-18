using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RealEstate.Application.DTOs.Owner;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services
{
    public interface IOwnerService
    {
        Task<OwnerResponseDto> CreateAsync(OwnerCreateDto dto, CancellationToken ct = default);
        Task<OwnerResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<OwnerResponseDto>> SearchAsync(string? name, int skip, int take, CancellationToken ct = default);
        Task<bool> UpdateAsync(Guid id, OwnerUpdateDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    }

    public sealed class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _repo;
        private readonly IMapper _mapper;

        public OwnerService(IOwnerRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OwnerResponseDto> CreateAsync(OwnerCreateDto dto, CancellationToken ct = default)
        {
            var entity = new Owner(dto.Name, dto.Address, dto.Photo);
            await _repo.CreateAsync(entity, ct);
            return _mapper.Map<OwnerResponseDto>(entity);
        }

        public async Task<OwnerResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            return entity is null ? null : _mapper.Map<OwnerResponseDto>(entity);
        }

        public async Task<IReadOnlyList<OwnerResponseDto>> SearchAsync(string? name, int skip, int take, CancellationToken ct = default)
        {
            var list = await _repo.SearchAsync(name, skip, take, ct);
            return _mapper.Map<IReadOnlyList<OwnerResponseDto>>(list);
        }

        public async Task<bool> UpdateAsync(Guid id, OwnerUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null) return false;
            entity.Update(dto.Name, dto.Address, dto.Photo);
            return await _repo.UpdateAsync(entity, ct);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            return _repo.DeleteAsync(id, ct);
        }
    }
}
