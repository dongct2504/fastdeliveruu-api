using FastDeliveruu.Application.Common.Behaviors;
using FastDeliveruu.Common.Constants;
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
            .Must(ValidateForRequest.BeValidPhoneNumber)
            .WithMessage(ErrorMessageConstants.PhoneValidator);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(128);

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.Role)
            .Must(ValidateForRequest.BeValidRole)
            .WithMessage(ErrorMessageConstants.RoleValidator);

        RuleFor(x => x.HouseNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.StreetName)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(x => x.CityId)
            .NotEmpty();

        RuleFor(x => x.DistrictId)
            .NotEmpty();

        RuleFor(x => x.WardId)
            .NotEmpty();
    }
}