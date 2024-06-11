using FluentValidation;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.UpdateCartItem;

public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(x => x.MenuItemId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .NotEmpty();
    }
}
