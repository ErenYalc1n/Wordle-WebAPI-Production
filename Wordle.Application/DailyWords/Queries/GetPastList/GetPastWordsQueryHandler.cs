using AutoMapper;
using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.DailyWords.DTOs;
using Wordle.Domain.DailyWords;

namespace Wordle.Application.DailyWords.Queries.GetPastList;

public class GetPastWordsQueryHandler : IRequestHandler<GetPastWordsQuery, DailyWordListResultDto>
{
    private readonly IDailyWordRepository _repository;
    private readonly IMapper _mapper;

    public GetPastWordsQueryHandler(IDailyWordRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DailyWordListResultDto> Handle(GetPastWordsQuery request, CancellationToken cancellationToken)
    {
        var allPastWords = await _repository.GetPastWordsAsync(request.Page, request.PageSize);
        var totalCount = await _repository.CountPastAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new DailyWordListResultDto
        {
            CurrentPage = request.Page,
            TotalPages = totalPages,
            Words = _mapper.Map<List<DailyWordListItemDto>>(allPastWords)
        };
    }
}
