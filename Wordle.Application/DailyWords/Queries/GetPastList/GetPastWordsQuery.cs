using MediatR;
using Wordle.Application.DailyWords.DTOs;

namespace Wordle.Application.DailyWords.Queries.GetPastList;

public class GetPastWordsQuery : IRequest<DailyWordListResultDto>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}
