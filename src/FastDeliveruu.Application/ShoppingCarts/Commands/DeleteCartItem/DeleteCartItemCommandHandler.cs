using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.DeleteCartItem;

public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, Result<int>>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteCartItemCommandHandler> _logger;

    public DeleteCartItemCommandHandler(
        ICacheService cacheService,
        ILogger<DeleteCartItemCommandHandler> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(
        DeleteCartItemCommand request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.CustomerCart}-{request.UserId}";

        List<ShoppingCartDto>? customerCartCache = await _cacheService
            .GetAsync<List<ShoppingCartDto>>(key, cancellationToken);

        if (customerCartCache == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CustomerCartEmpty} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CustomerCartEmpty));
        }

        ShoppingCartDto? cartItem = customerCartCache
            .FirstOrDefault(sc => sc.Id == request.Id);

        if (cartItem == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CartItemNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityDuplicate));
        }

        customerCartCache.Remove(cartItem);

        await _cacheService.SetAsync(key, customerCartCache, CacheOptions.CartExpiration, cancellationToken);

        return customerCartCache.Sum(cart => cart.Quantity);
    }
}
