using AutoMapper;
using Wordle.Application.DailyWords.DTOs;
using Wordle.Domain.DailyWords;

namespace Wordle.Application.DailyWords.Mappings;

public class DailyWordMappingProfile : Profile
{
    public DailyWordMappingProfile()
    {
        CreateMap<DailyWord, DailyWordListItemDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Date)));

        CreateMap<DailyWord, GetDailyWordDto>();

        CreateMap<DailyWord, SearchDailyWordDto>()
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Date)));
    }
}
