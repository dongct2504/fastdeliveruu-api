using FluentValidation;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.UpdateShoppingCart;

public class UpdateShoppingCartCommandValidator : AbstractValidator<UpdateShoppingCartCommand>
{
    public UpdateShoppingCartCommandValidator()
    {
        RuleFor(x => x.MenuItemId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .NotEmpty();
    }
}
