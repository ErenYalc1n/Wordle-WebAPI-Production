using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Common;

namespace Wordle.Application.Users.Commands.Update;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UpdateUserCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
        {
            _logger.LogWarning("UpdateUser: JWT'den kullanıcı ID çözülemedi.");
            throw new UnauthorizedAppException("Kimlik doğrulama başarısız.");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("UpdateUser: Kullanıcı bulunamadı. UserId: {UserId}", userId);
            throw new UnauthorizedAppException("Kullanıcı bulunamadı.");
        }

        user.Nickname = request.Nickname;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("UpdateUser: Kullanıcı bilgileri güncellendi. UserId: {UserId}", user.Id);

        return Unit.Value;
    }
}
