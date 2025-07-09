using FluentValidation;

namespace Wordle.Application.DailyWords.Commands.Update;

public class UpdateDailyWordCommandValidator : AbstractValidator<UpdateDailyWordCommand>
{
    public UpdateDailyWordCommandValidator()
    {
        RuleFor(x => x.DailyWord.Word)
            .NotEmpty().WithMessage("Kelime boş olamaz.")
            .Length(5).WithMessage("Kelime 5 harfli olmalıdır.")
            .Matches("^[a-zA-ZçÇğĞıİöÖşŞüÜ]+$").WithMessage("Kelime sadece harf içermelidir.");

        RuleFor(x => x.DailyWord.NewDate)
            .Must(date => date > DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3).Date))
            .WithMessage("Sadece gelecekteki kelimeler güncellenebilir.");
    }
}
