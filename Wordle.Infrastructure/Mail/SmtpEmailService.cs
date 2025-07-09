using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Wordle.Application.Common.Interfaces;
using Wordle.Infrastructure.Mail;

namespace Wordle.Infrastructure.Mail;

public class SmtpEmailService : IEMailService
{
    private readonly SmtpSettings _settings;

    public SmtpEmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_settings.Username),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        using var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}
