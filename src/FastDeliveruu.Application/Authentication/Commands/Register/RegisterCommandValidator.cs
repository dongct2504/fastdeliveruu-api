using FluentValidation;

namespace FastDeliveruu.Application.Authentication.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Email).NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}