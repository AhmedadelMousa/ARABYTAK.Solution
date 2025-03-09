using Arabytak.Core.Entities;
using ARABYTAK.APIS.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace ARABYTAK.APIS.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Car, CarDto>()
                .ForMember(c => c.DealershipName, d => d.MapFrom(s => s.dealership.Name))
                .ForMember(c => c.brand, d => d.MapFrom(s => s.brand.Name))
                .ForMember(c => c.model, d => d.MapFrom(s => s.model.Name))
                .ForMember(c=>c.Id,d=>d.MapFrom(s=>s.Id))
              .ForMember(c => c.Url, d => d.MapFrom(s => s.Url.Select(u => new CarPictureDto { Url = u.PictureUrl }).ToList()))
            .ForMember(dest => dest.Url, opt => opt.MapFrom<PictureUrlResolver>());
            //.ForMember(c=>c.Url,d=>d.MapFrom(s=>s.Url.SelectMany(u=>u.PictureUrl )))
            CreateMap<SpecNewCar, SpecNewCarDto>();
            CreateMap<SpecUsedCar, SpecUsedCarDto>();
            CreateMap<CarPictureUrl, CarPictureDto>();
            CreateMap<Car, CarListDto>()
                .ForMember(c => c.DealershipName, d => d.MapFrom(s => s.dealership.Name))
                .ForMember(c => c.Price, d => d.MapFrom(s => s.Price))
                .ForMember(c => c.condition, d => d.MapFrom(s => s.condition))
                .ForMember(c=>c.CarName,d=>d.MapFrom(s=>$"{s.brand.Name} {s.model.Name}"))
                .ForMember(c => c.Url, d => d.MapFrom(s => s.Url.Select(u => new CarPictureDto { Url = u.PictureUrl }).ToList()))
            .ForMember(dest => dest.Url, opt => opt.MapFrom<PictureUrlResolver>());

            CreateMap<RoleDto, IdentityRole>();
            CreateMap<AdvertismentDto, Advertisement>();
               
        }
    }
}
