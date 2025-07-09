using MediatR;
using Microsoft.Extensions.Logging;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Users.DTOs;
using Wordle.Domain.Common;

namespace Wordle.Application.Users.Commands.Login;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<LoginUserCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        ILogger<LoginUserCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdentifierAsync(request.Identifier);

        if (user is null)
        {
            _logger.LogWarning("Login failed: kullanıcı bulunamadı. Identifier: {Identifier}", request.Identifier);
            throw new UnauthorizedAppException("Kullanıcı bulunamadı.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: şifre hatalı. UserId: {UserId}, Email: {Email}", user.Id, user.Email);
            throw new UnauthorizedAppException("Şifre hatalı.");
        }

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = _tokenService.CreateToken(user);

        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Login success: Kullanıcı giriş yaptı. UserId: {UserId}, Email: {Email}", user.Id, user.Email);

        return new LoginResultDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }
}
