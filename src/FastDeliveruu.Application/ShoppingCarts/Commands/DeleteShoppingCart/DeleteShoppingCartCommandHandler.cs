using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.ShoppingCarts;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.DeleteShoppingCart;

public class DeleteShoppingCartCommandHandler : IRequestHandler<DeleteShoppingCartCommand, Result>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public DeleteShoppingCartCommandHandler(IShoppingCartRepository shoppingCartRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
    }

    public async Task<Result> Handle(DeleteShoppingCartCommand request, CancellationToken cancellationToken)
    {
        var spec = new CartByUserIdAndMenuItemIdSpecification(request.LocalUserId, request.MenuItemId);
        ShoppingCart? shoppingCart = await _shoppingCartRepository.GetWithSpecAsync(spec, asNoTracking: true);
        if (shoppingCart == null)
        {
            string message = "Shopping Cart not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        await _shoppingCartRepository.DeleteAsync(shoppingCart);

        return Result.Ok();
    }
}
