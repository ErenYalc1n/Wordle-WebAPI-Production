using MediatR;
using Microsoft.Extensions.Logging;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Common;
using Wordle.Domain.Users;

namespace Wordle.Application.Users.Commands.Delete;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        ILogger<DeleteUserCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        _logger.LogInformation("DeleteUser: Kullanıcı silme işlemi başlatıldı. UserId: {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("DeleteUser: Kullanıcı bulunamadı. UserId: {UserId}", userId);
            throw new NotFoundException("Kullanıcı bulunamadı.");
        }

        await _userRepository.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("DeleteUser: Kullanıcı başarıyla silindi. UserId: {UserId}", userId);

        return Unit.Value;
    }
}
