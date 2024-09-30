using FluentValidation;

namespace FastDeliveruu.Application.MenuVariantInventories.Commands.UpdateMenuVariantInventory;

public class UpdateMenuVariantInventoryCommandValidator : AbstractValidator<UpdateMenuVariantInventoryCommand>
{
    public UpdateMenuVariantInventoryCommandValidator()
    {
        RuleFor(x => x.MenuVariantId)
            .NotEmpty();

        RuleFor(x => x.QuantityAvailable)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.QuantityReserved)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);
    }
}
