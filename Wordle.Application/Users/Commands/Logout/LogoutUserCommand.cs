using MediatR;

namespace Wordle.Application.Users.Commands.Logout
{
    public class LogoutUserCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}
