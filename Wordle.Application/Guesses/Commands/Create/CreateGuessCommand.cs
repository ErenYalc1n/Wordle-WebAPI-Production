using MediatR;
using Wordle.Application.DTOs;

namespace Wordle.Application.Guesses.Commands.Create;

public class CreateGuessCommand : IRequest<GuessResponseDto>
{
    public string GuessText { get; set; } = string.Empty;
}
