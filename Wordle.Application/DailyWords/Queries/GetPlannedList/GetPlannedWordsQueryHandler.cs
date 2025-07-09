using AutoMapper;
using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.DailyWords.DTOs;
using Wordle.Domain.DailyWords;

namespace Wordle.Application.DailyWords.Queries.GetPlannedList;

public class GetPlannedWordsQueryHandler : IRequestHandler<GetPlannedWordsQuery, DailyWordListResultDto>
{
    private readonly IDailyWordRepository _repository;
    private readonly IMapper _mapper;

    public GetPlannedWordsQueryHandler(IDailyWordRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DailyWordListResultDto> Handle(GetPlannedWordsQuery request, CancellationToken cancellationToken)
    {
        var allPlanned = await _repository.GetPlannedWordsAsync(request.Page, request.PageSize);
        var totalCount = await _repository.CountPlannedAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new DailyWordListResultDto
        {
            CurrentPage = request.Page,
            TotalPages = totalPages,
            Words = _mapper.Map<List<DailyWordListItemDto>>(allPlanned)
        };
    }
}
