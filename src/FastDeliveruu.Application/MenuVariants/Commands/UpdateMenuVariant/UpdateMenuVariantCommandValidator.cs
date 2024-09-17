using FluentValidation;

namespace FastDeliveruu.Application.MenuVariants.Commands.UpdateMenuVariant;

public class UpdateMenuVariantCommandValidator : AbstractValidator<UpdateMenuVariantCommand>
{
    public UpdateMenuVariantCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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
    }
}
