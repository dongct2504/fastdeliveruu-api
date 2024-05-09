using FastDeliveruu.Application.Common.Behaviors;
using FluentValidation;

namespace FastDeliveruu.Application.Authentication.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(ValidateForRequest.BeValidPhoneNumber).WithMessage("Invalid phone number.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(128);

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.Role)
            .Must(ValidateForRequest.BeValidRole).WithMessage("Role must be Customer, Staff or Admin.");
    }
}