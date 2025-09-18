using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RealEstate.Application.DTOs.PropertyTrace;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Domain.ValueObjects;
using DomainTrace = RealEstate.Domain.Entities.PropertyTrace;

namespace RealEstate.Application.Services
{
    public interface IPropertyTraceService
    {
        Task<PropertyTraceResponseDto> CreateAsync(PropertyTraceCreateDto dto, CancellationToken ct = default);
        Task<IReadOnlyList<PropertyTraceResponseDto>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default);
    }

    public sealed class PropertyTraceService : IPropertyTraceService
    {
        private readonly IPropertyTraceRepository _repo;
        private readonly IMapper _mapper;

        public PropertyTraceService(IPropertyTraceRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PropertyTraceResponseDto> CreateAsync(PropertyTraceCreateDto dto, CancellationToken ct = default)
        {
            var value = new Price(dto.Value, dto.Currency);
            var entity = new DomainTrace(dto.PropertyId, dto.DateUtc, dto.Description, value);
            await _repo.CreateAsync(entity, ct);
            return _mapper.Map<PropertyTraceResponseDto>(entity);
        }

        public async Task<IReadOnlyList<PropertyTraceResponseDto>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default)
        {
            var list = await _repo.GetByPropertyIdAsync(propertyId, ct);
            return _mapper.Map<IReadOnlyList<PropertyTraceResponseDto>>(list);
        }
    }
}


