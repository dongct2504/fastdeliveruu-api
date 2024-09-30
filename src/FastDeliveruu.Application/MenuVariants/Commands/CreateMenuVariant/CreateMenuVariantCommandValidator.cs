using FluentValidation;

namespace FastDeliveruu.Application.MenuVariants.Commands.CreateMenuVariant;

public class CreateMenuVariantCommandValidator : AbstractValidator<CreateMenuVariantCommand>
{
    public CreateMenuVariantCommandValidator()
    {
        RuleFor(x => x.MenuItemId)
            .NotEmpty();

        RuleFor(x => x.VarietyName)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThanOrEqualTo(1000);

        RuleFor(x => x.DiscountPercent)
            .NotEmpty()
            .InclusiveBetween(0, 1);

        RuleFor(x => x.ImageFile)
            .NotEmpty();
    }
}
