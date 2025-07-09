using MediatR;
using Microsoft.Extensions.Logging;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Common;

namespace Wordle.Application.Users.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IEMailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IEMailService emailService,
        ILogger<ForgotPasswordCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
        {
            _logger.LogWarning("ForgotPassword: Email adresi bulunamadı. Email: {Email}", request.Email);
            throw new NotFoundException("Bu email adresine sahip bir kullanıcı bulunamadı.");
        }

        user.PasswordResetCode = new Random().Next(100000, 999999).ToString();
        user.PasswordResetExpiresAt = DateTime.UtcNow.AddMinutes(15);

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            var body = $"Şifre sıfırlama kodunuz: {user.PasswordResetCode}";
            await _emailService.SendEmailAsync(user.Email, "Şifre Sıfırlama", body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Şifre sıfırlama maili gönderilemedi. UserId: {UserId}", user.Id);
        }


        _logger.LogInformation("ForgotPassword: Şifre sıfırlama kodu gönderildi. UserId: {UserId}, Email: {Email}", user.Id, user.Email);

        return Unit.Value;
    }
}
