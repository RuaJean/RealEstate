using AutoMapper;
using RealEstate.Application.DTOs.Owner;
using RealEstate.Application.DTOs.Property;
using RealEstate.Application.DTOs.PropertyImage;
using RealEstate.Application.DTOs.PropertyTrace;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Owner, OwnerResponseDto>().ReverseMap();

            CreateMap<Property, PropertyResponseDto>()
                .ForMember(d => d.Street, o => o.MapFrom(s => s.Address.Street))
                .ForMember(d => d.City, o => o.MapFrom(s => s.Address.City))
                .ForMember(d => d.State, o => o.MapFrom(s => s.Address.State))
                .ForMember(d => d.Country, o => o.MapFrom(s => s.Address.Country))
                .ForMember(d => d.ZipCode, o => o.MapFrom(s => s.Address.ZipCode))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Price.Amount))
                .ForMember(d => d.Currency, o => o.MapFrom(s => s.Price.Currency));

            CreateMap<PropertyImage, PropertyImageResponseDto>();
            CreateMap<PropertyTrace, PropertyTraceResponseDto>()
                .ForMember(d => d.Value, o => o.MapFrom(s => s.Value.Amount))
                .ForMember(d => d.Currency, o => o.MapFrom(s => s.Value.Currency));
        }
    }
}
