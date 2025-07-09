using MediatR;

namespace Wordle.Application.Users.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest
{
    public string Email { get; set; } = string.Empty;
}
