using MediatR;
using Wordle.Application.Users.DTOs;

namespace Wordle.Application.Users.Commands.VerifyEmail;

public class VerifyEmailCommand : IRequest<AuthResultDto>
{
    public string Code { get; set; } = string.Empty;
}
