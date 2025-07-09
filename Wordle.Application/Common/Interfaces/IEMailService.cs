namespace Wordle.Application.Common.Interfaces;

public interface IEMailService
{
    Task SendEmailAsync(string to, string subject, string body);
}
