using MediatR;
using Microsoft.Extensions.Logging;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Users.DTOs;
using Wordle.Domain.Common;

namespace Wordle.Application.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<RefreshTokenCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenPreview = request.RefreshToken?.Substring(0, Math.Min(10, request.RefreshToken.Length)) ?? "null";

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            _logger.LogWarning("RefreshToken: Null veya boş refresh token alındı.");
            throw new UnauthorizedAppException("Geçersiz token.");
        }

        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);

        if (user is null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("RefreshToken: Geçersiz veya süresi dolmuş token ile istek alındı. Token başlangıcı: {TokenPreview}", tokenPreview);
            throw new UnauthorizedAppException("Geçersiz ya da süresi dolmuş refresh token.");
        }

        var tokens = _tokenService.CreateToken(user);

        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RefreshToken: Token başarıyla yenilendi. UserId: {UserId}, Email: {Email}", user.Id, user.Email);

        return new LoginResultDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        };
    }
}
