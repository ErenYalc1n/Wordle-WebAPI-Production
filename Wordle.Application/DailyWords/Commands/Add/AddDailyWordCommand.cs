using MediatR;
using Wordle.Application.DailyWords.DTOs;

namespace Wordle.Application.DailyWords.Commands.Add;

public class AddDailyWordCommand : IRequest<Guid>
{
    public AddDailyWordDto DailyWord { get; set; } = default!;
}
