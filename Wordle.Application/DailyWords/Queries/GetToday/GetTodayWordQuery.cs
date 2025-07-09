using MediatR;
using Wordle.Application.DailyWords.DTOs;

namespace Wordle.Application.DailyWords.Queries.GetToday;

public class GetTodayWordQuery : IRequest<GetDailyWordDto?>
{
}
