using FastDeliveruu.Application.Common.Behaviors;
using FastDeliveruu.Common.Constants;
using FluentValidation;

namespace FastDeliveruu.Application.Authentication.Commands.ShipperRegister;

public class ShipperRegisterCommandValidator : AbstractValidator<ShipperRegisterCommand>
{
    public ShipperRegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.CitizenIdentification)
            .NotEmpty()
            .MaximumLength(12);

        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(ValidateForRequest.BeValidPhoneNumber)
            .WithMessage(ErrorMessageConstants.PhoneValidator);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Password)
            .NotEmpty();

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
