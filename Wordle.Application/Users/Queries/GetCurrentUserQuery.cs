using MediatR;
using Wordle.Application.Users.DTOs;

namespace Wordle.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<CurrentUserDto>
{
}
