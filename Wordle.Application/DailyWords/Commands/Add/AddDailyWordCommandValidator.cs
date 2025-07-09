using FluentValidation;

namespace Wordle.Application.DailyWords.Commands.Add;

public class AddDailyWordCommandValidator : AbstractValidator<AddDailyWordCommand>
{
    public AddDailyWordCommandValidator()
    {
        RuleFor(x => x.DailyWord.Word)
            .NotEmpty().WithMessage("Kelime boş olamaz.")
            .Length(5).WithMessage("Kelime 5 harfli olmalıdır.")
            .Matches("^[a-zA-ZçÇğĞıİöÖşŞüÜ]+$").WithMessage("Kelime sadece harf içermelidir.");

        RuleFor(x => x.DailyWord.Date)
            .Must(BeAFutureOrTodayDate).WithMessage("Bugün ya da daha sonraki bir tarih olmalıdır.");
    }

    private bool BeAFutureOrTodayDate(DateOnly date)
    {
        return date >= DateOnly.FromDateTime(DateTime.Now); 
    }
}
