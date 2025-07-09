using MediatR;
using Wordle.Application.DailyWords.DTOs;

namespace Wordle.Application.DailyWords.Commands.Update;

public class UpdateDailyWordCommand : IRequest
{
    public UpdateDailyWordDto DailyWord { get; set; } = default!;
}
