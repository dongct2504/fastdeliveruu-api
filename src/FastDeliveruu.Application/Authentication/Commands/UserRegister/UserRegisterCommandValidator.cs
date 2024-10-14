using FastDeliveruu.Application.Common.Behaviors;
using FluentValidation;

namespace FastDeliveruu.Application.Authentication.Commands.UserRegister;

public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
{
    public UserRegisterCommandValidator()
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

        RuleFor(x => x.Address)
            .NotNull()
            .MaximumLength(60);

        RuleFor(x => x.CityId)
            .NotEmpty();

        RuleFor(x => x.DistrictId)
            .NotEmpty();

        RuleFor(x => x.WardId)
            .NotEmpty();
    }
}