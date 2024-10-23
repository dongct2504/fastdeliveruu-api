using FastDeliveruu.Application.Common.Behaviors;
using FastDeliveruu.Application.Common.Constants;
using FluentValidation;

namespace FastDeliveruu.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(126);

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(ValidateForRequest.BeValidPhoneNumber)
            .WithMessage(ErrorMessageConstants.PhoneValidator);;

        RuleFor(x => x.HouseNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.StreetName)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(x => x.WardId)
            .NotEmpty();

        RuleFor(x => x.DistrictId)
            .NotEmpty();

        RuleFor(x => x.CityId)
            .NotEmpty();

        RuleFor(x => x.ImageFile)
            .NotEmpty();
    }
}