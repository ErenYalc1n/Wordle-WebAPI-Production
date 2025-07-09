using FluentValidation;

namespace Wordle.Application.Users.Commands.Register
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalı.")
                .MaximumLength(50).WithMessage("Şifre en fazla 50 karakter olmalı.");

            RuleFor(x => x.Nickname)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
                .MaximumLength(20).WithMessage("Kullanıcı adı en fazla 20 karakter olabilir.")
                .MinimumLength(3).WithMessage("Kullanıcı adı en fazla 3 karakter olmalı.");

            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("İsim en fazla 50 karakter olabilir.")
                .MinimumLength(3).WithMessage("İsim en az 3 karakter olabilir.");

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Soyisim en fazla 50 karakter olabilir.")
                .MinimumLength(3).WithMessage("Soyisim en az 3 karakter olabilir.");

            RuleFor(x => x.IsKvkkAccepted)
                .Equal(true).WithMessage("KVKK onayı zorunludur.");
        }
    }
}
