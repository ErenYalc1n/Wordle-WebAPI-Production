using MediatR;

namespace Wordle.Application.Users.Commands.Update;

public class UpdateUserCommand : IRequest
{
    public string Nickname { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}
