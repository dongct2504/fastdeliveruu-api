using FastDeliveruu.Application.MenuItemInventories.Commands.CreateMenuItemInventory;
using FluentValidation;

namespace FastDeliveruu.Application.MenuItemInventories.Commands.UpdateMenuItemInventory;

public class UpdateMenuItemInventoryCommandValidator : AbstractValidator<UpdateMenuItemInventoryCommand>
{
    public UpdateMenuItemInventoryCommandValidator()
    {
        RuleFor(x => x.MenuItemId)
            .NotEmpty();

        RuleFor(x => x.QuantityAvailable)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.QuantityReserved)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);
    }
}
