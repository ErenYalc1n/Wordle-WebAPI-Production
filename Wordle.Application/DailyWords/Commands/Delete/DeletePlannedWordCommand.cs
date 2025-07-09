using MediatR;

namespace Wordle.Application.DailyWords.Commands.Delete;

public class DeletePlannedWordCommand : IRequest
{
    public DateOnly Date { get; set; }
}
