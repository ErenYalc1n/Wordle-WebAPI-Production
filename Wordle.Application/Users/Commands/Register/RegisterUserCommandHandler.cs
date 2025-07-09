using MediatR;
using Microsoft.Extensions.Logging;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Users.Commands.Register;
using Wordle.Application.Users.DTOs;
using Wordle.Domain.Common;
using Wordle.Domain.Users;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEMailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IEMailService emailService,
        IPasswordHasher passwordHasher,
        ILogger<RegisterUserCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailService = emailService;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Register failed: E-posta zaten kayıtlı. Email: {Email}", request.Email);
            throw new ConflictException("Bu e-posta zaten kayıtlı.");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            Nickname = request.Nickname,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = Role.UnverifiedPlayer,
            IsKvkkAccepted = request.IsKvkkAccepted,
            IsEmailConfirmed = false,
            EmailVerificationCode = new Random().Next(100000, 999999).ToString(),
            EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        await _userRepository.AddAsync(user);

        var tokens = _tokenService.CreateToken(user);
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Register success: Yeni kullanıcı kaydı oluşturuldu. UserId: {UserId}, Email: {Email}", user.Id, user.Email);

        try
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "Wordle Mail Adresi Onayı",
                $"Doğrulama Kodunuz: {user.EmailVerificationCode}");

            _logger.LogInformation("Register: Doğrulama e-postası gönderildi. Email: {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Register: Doğrulama maili gönderilirken hata oluştu. UserId: {UserId}", user.Id);
        }

        return new AuthResultDto
        {
            Token = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }
}
