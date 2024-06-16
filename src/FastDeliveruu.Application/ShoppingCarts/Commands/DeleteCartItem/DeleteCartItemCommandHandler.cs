using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.DeleteCartItem;

public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, Result<int>>
{
    private readonly ICacheService _cacheService;

    public DeleteCartItemCommandHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<Result<int>> Handle(
        DeleteCartItemCommand request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.CustomerCart}-{request.UserId}";

        List<ShoppingCart>? customerCartCache = await _cacheService
            .GetAsync<List<ShoppingCart>>(key, cancellationToken);

        if (customerCartCache == null)
        {
            string message = "The customer's cart is already empty";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        ShoppingCart? shoppingCartRemove = customerCartCache
            .Where(sc => sc.AppUserId == request.UserId && sc.MenuItemId == request.MenuItemId)
            .FirstOrDefault();

        if (shoppingCartRemove == null)
        {
            string message = "Cart not found";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        customerCartCache.Remove(shoppingCartRemove);

        await _cacheService.SetAsync(key, customerCartCache, CacheOptions.CartExpiration, cancellationToken);

        return customerCartCache.Sum(cart => cart.Quantity);
    }
}
