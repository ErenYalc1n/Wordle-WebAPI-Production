using MediatR;

namespace Wordle.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<string>
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
