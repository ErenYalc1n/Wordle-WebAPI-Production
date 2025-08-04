using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Users.DTOs;
using Wordle.Domain.Common;
using Wordle.Domain.Users;

namespace Wordle.Application.Users.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;
    private readonly ILogger<VerifyEmailCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyEmailCommandHandler(
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService,
        ILogger<VerifyEmailCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            _logger.LogWarning("VerifyEmail: JWT token'dan userId alınamadı.");
            throw new UnauthorizedAppException("Geçersiz kullanıcı.");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("VerifyEmail: Kullanıcı bulunamadı. UserId: {UserId}", userId);
            throw new UnauthorizedAppException("Kullanıcı bulunamadı.");
        }

        if (user.EmailVerificationCode != request.Code || user.EmailVerificationExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("VerifyEmail: Kod geçersiz veya süresi dolmuş. UserId: {UserId}, Email: {Email}", user.Id, user.Email);
            throw new ValidationAppException("Geçersiz ya da süresi dolmuş doğrulama kodu.");
        }

        if (user.Role == Role.UnverifiedPlayer)
            user.Role = Role.Player;
        user.IsEmailConfirmed = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationExpiresAt = null;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = _tokenService.CreateToken(user);
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;

        _logger.LogInformation("VerifyEmail: E-posta başarıyla doğrulandı. UserId: {UserId}, Email: {Email}", user.Id, user.Email);

        return new AuthResultDto
        {
            Token = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }
}
