using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result>
{
    private readonly ICacheService _cacheService;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public UpdateCartItemCommandHandler(
        IMapper mapper,
        ICacheService cacheService,
        IMenuItemRepository menuItemRepository)
    {
        _mapper = mapper;
        _cacheService = cacheService;
        _menuItemRepository = menuItemRepository;
    }

    public async Task<Result> Handle(
        UpdateCartItemCommand request,
        CancellationToken cancellationToken)
    {
        MenuItem? menuItem = await _menuItemRepository.GetAsync(request.MenuItemId);
        if (menuItem == null)
        {
            string message = "MenuItem not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        string key = $"{CacheConstants.CustomerCart}-{request.LocalUserId}";

        List<ShoppingCart>? customerCartCache = await _cacheService.GetAsync<List<ShoppingCart>>(key, cancellationToken);
        if (customerCartCache == null)
        {
            ShoppingCart cartItem = _mapper.Map<ShoppingCart>(request);
            cartItem.MenuItem = menuItem;

            List<ShoppingCart> cartItems = new List<ShoppingCart>
            {
                cartItem
            };

            await _cacheService.SetAsync(key, cartItems, CacheOptions.CartExpiration, cancellationToken);

            return Result.Ok();
        }

        ShoppingCart? shoppingCartUpdate = customerCartCache
            .Where(sc => sc.LocalUserId == request.LocalUserId && sc.MenuItemId == request.MenuItemId)
            .FirstOrDefault();

        if (shoppingCartUpdate == null)
        {
            ShoppingCart newCartItem = _mapper.Map<ShoppingCart>(request);
            newCartItem.MenuItem = menuItem;
            customerCartCache.Add(newCartItem);
        }
        else
        {
            _mapper.Map(request, shoppingCartUpdate);
        }

        await _cacheService.SetAsync(key, customerCartCache, CacheOptions.CartExpiration, cancellationToken);

        return Result.Ok();
    }
}
