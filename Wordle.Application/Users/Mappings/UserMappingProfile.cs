using AutoMapper;
using Wordle.Application.Users.DTOs;
using Wordle.Domain.Users;

namespace Wordle.Application.Users.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, CurrentUserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ReverseMap();
    }
}
