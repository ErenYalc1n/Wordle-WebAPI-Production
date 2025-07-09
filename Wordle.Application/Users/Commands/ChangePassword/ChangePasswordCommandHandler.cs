using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Common;

namespace Wordle.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        IPasswordHasher passwordHasher,
        ILogger<ChangePasswordCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            _logger.LogWarning("ChangePassword: JWT içinden kullanıcı ID alınamadı.");
            throw new UnauthorizedAppException("Kullanıcı doğrulanamadı.");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("ChangePassword: Kullanıcı bulunamadı. UserId: {UserId}", userId);
            throw new NotFoundException("Kullanıcı bulunamadı.");
        }

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            _logger.LogWarning("ChangePassword: Kullanıcı mevcut şifreyi yanlış girdi. UserId: {UserId}", user.Id);
            throw new UnauthorizedAppException("Mevcut şifre hatalı.");
        }

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ChangePassword: Şifre başarıyla değiştirildi. UserId: {UserId}", user.Id);

        return "Şifre başarıyla değiştirildi.";
    }
}
