using MediatR;
using Wordle.Application.DailyWords.DTOs;

namespace Wordle.Application.DailyWords.Queries.GetPlannedList;

public class GetPlannedWordsQuery : IRequest<DailyWordListResultDto>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    
}
