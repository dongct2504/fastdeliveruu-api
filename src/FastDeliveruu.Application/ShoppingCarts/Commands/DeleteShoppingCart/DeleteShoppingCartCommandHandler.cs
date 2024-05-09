using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
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
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            Where = sc => sc.LocalUserId == request.LocalUserId && sc.MenuItemId == request.MenuItemId
        };
        ShoppingCart? shoppingCart = await _shoppingCartRepository.GetAsync(options);
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
