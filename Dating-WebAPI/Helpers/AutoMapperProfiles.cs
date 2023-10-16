using AutoMapper;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Extensions;

namespace Dating_WebAPI.Helpers;
public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MembersDTO>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).ImageUrl))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDTO>();
    }
}
