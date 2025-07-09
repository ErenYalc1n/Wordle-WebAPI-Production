using MediatR;
using Wordle.Application.Users.DTOs;

namespace Wordle.Application.Users.Commands.Login
{
    public class LoginUserCommand : IRequest<LoginResultDto>
    {
        public string Identifier { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
