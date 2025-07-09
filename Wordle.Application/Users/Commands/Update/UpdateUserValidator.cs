using FluentValidation;

namespace Wordle.Application.Users.Commands.Update
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Nickname)
                .NotEmpty().WithMessage("Nickname boş olamaz.")
                .MinimumLength(3).WithMessage("Minimum 3 karakter olmalı.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("İsim boş olamaz.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyisim boş olamaz.");
        }
    }
}
