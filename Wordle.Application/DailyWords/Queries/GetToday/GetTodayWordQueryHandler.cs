using AutoMapper;
using MediatR;
using Wordle.Domain.DailyWords;
using Wordle.Application.DailyWords.DTOs;
using Wordle.Application.Common.Interfaces;

namespace Wordle.Application.DailyWords.Queries.GetToday;

public class GetTodayWordQueryHandler : IRequestHandler<GetTodayWordQuery, GetDailyWordDto?>
{
    private readonly IDailyWordRepository _repository;
    private readonly IMapper _mapper;

    public GetTodayWordQueryHandler(IDailyWordRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<GetDailyWordDto?> Handle(GetTodayWordQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        var word = await _repository.GetTodayWordAsync(today);

        return word is null ? null : _mapper.Map<GetDailyWordDto>(word);
    }
}
