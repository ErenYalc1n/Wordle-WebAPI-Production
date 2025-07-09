using MediatR;
using Microsoft.Extensions.Logging;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Common;

namespace Wordle.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<ResetPasswordCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
        {
            _logger.LogWarning("ResetPassword: Kullanıcı bulunamadı. Email: {Email}", request.Email);
            throw new UnauthorizedAppException("Kullanıcı bulunamadı.");
        }

        if (user.PasswordResetCode != request.Code || user.PasswordResetExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("ResetPassword: Kod geçersiz veya süresi dolmuş. Email: {Email}", user.Email);
            throw new UnauthorizedAppException("Kod geçersiz veya süresi dolmuş.");
        }

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.PasswordResetCode = null;
        user.PasswordResetExpiresAt = null;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ResetPassword: Şifre başarıyla sıfırlandı. UserId: {UserId}, Email: {Email}", user.Id, user.Email);

        return "Şifre başarıyla yenilendi.";
    }
}
