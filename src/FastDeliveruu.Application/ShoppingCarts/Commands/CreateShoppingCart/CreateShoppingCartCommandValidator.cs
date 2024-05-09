using FluentValidation;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.CreateShoppingCart;

public class CreateShoppingCartCommandValidator : AbstractValidator<CreateShoppingCartCommand>
{
    public CreateShoppingCartCommandValidator()
    {
        RuleFor(x => x.LocalUserId)
            .NotEmpty();

        RuleFor(x => x.MenuItemId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .NotEmpty();
    }
}
