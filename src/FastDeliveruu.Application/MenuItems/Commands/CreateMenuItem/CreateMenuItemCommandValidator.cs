using FluentValidation;

namespace FastDeliveruu.Application.MenuItems.Commands.CreateMenuItem;

public class CreateMenuItemCommandValidator : AbstractValidator<CreateMenuItemCommand>
{
    public CreateMenuItemCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty();

        RuleFor(x => x.GenreId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.Inventory)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThanOrEqualTo(1000);

        RuleFor(x => x.DiscountPercent)
            .NotEmpty()
            .InclusiveBetween(0, 1);
    }
}
