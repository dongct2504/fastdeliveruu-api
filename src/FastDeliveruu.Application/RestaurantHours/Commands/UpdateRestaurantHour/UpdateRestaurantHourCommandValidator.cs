using FluentValidation;

namespace FastDeliveruu.Application.RestaurantHours.Commands.UpdateRestaurantHour;

public class UpdateRestaurantHourCommandValidator : AbstractValidator<UpdateRestaurantHourCommand>
{
    public UpdateRestaurantHourCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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
