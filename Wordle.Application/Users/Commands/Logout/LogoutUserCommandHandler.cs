using MediatR;
using Microsoft.Extensions.Logging;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Common;

namespace Wordle.Application.Users.Commands.Logout;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LogoutUserCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutUserCommandHandler(
        IUserRepository userRepository,
        ILogger<LogoutUserCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            _logger.LogWarning("Logout failed: kullanıcı bulunamadı. UserId: {UserId}", request.UserId);
            throw new UnauthorizedAppException("Kullanıcı bulunamadı.");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Logout success: Kullanıcı çıkış yaptı. UserId: {UserId}", user.Id);

        return Unit.Value;
    }
}
