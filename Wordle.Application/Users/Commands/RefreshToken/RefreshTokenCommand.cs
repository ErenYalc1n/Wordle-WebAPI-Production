using MediatR;
using Wordle.Application.Users.DTOs;

namespace Wordle.Application.Users.Commands.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResultDto>;
}
