using FluentValidation;

namespace FastDeliveruu.Application.RestaurantHours.Commands.CreateRestaurantHour;

public class CreateRestaurantHourCommandValidator : AbstractValidator<CreateRestaurantHourCommand>
{
    public CreateRestaurantHourCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty();

        RuleFor(x => x.WeekenDay)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .NotEmpty();
    }
}
