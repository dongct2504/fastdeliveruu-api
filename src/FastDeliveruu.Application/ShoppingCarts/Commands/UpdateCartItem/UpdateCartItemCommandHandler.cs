using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.MenuVariants;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result<int>>
{
    private readonly ICacheService _cacheService;
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCartItemCommandHandler(
        IMapper mapper,
        ICacheService cacheService,
        IFastDeliveruuUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(
        UpdateCartItemCommand request,
        CancellationToken cancellationToken)
    {
        MenuItem? menuItem = await _unitOfWork.MenuItems.GetAsync(request.MenuItemId);
        if (menuItem == null)
        {
            string message = "MenuItem not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        // check if menu variant provided
        MenuVariant? menuVariant = null;
        if (request.MenuVariantId.HasValue)
        {
            var spec = new MenuVariantIdExistInMenuItemSpecification(request.MenuItemId, request.MenuVariantId);
            menuVariant = await _unitOfWork.MenuVariants.GetWithSpecAsync(spec);

            if (menuVariant == null)
            {
                string message = "MenuVariant does not exist in the MenuItem.";
                Log.Warning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new NotFoundError(message));
            }
        }

        string key = $"{CacheConstants.CustomerCart}-{request.AppUserId}";

        List<ShoppingCart>? customerCartCache = await _cacheService.GetAsync<List<ShoppingCart>>(key, cancellationToken);

        if (customerCartCache == null)
        {
            // cart doesn't exist yet, create a new one
            ShoppingCart cartItem = _mapper.Map<ShoppingCart>(request);
            cartItem.MenuItem = menuItem;

            if (menuVariant != null)
            {
                cartItem.MenuVariant = menuVariant;
            }

            List<ShoppingCart> cartItems = new List<ShoppingCart>
            {
                cartItem
            };

            await _cacheService.SetAsync(key, cartItems, CacheOptions.CartExpiration, cancellationToken);

            return cartItem.Quantity;
        }

        // check if the item already exists in the cart (check both menu item and menu variant)
        ShoppingCart? shoppingCartUpdate = customerCartCache
            .FirstOrDefault(sc => 
                sc.AppUserId == request.AppUserId && 
                sc.MenuItemId == request.MenuItemId &&
                (!request.MenuVariantId.HasValue || sc.MenuVariantId == request.MenuVariantId));

        if (shoppingCartUpdate == null) // item doesn't exist, hence create a new item and add it to the cart
        {
            ShoppingCart newCartItem = _mapper.Map<ShoppingCart>(request);
            newCartItem.MenuItem = menuItem;

            if (menuVariant != null)
            {
                newCartItem.MenuVariant = menuVariant;
            }

            customerCartCache.Add(newCartItem);
        }
        else // item exist
        {
            shoppingCartUpdate.Quantity += request.Quantity;
        }

        await _cacheService.SetAsync(key, customerCartCache, CacheOptions.CartExpiration, cancellationToken);

        return customerCartCache.Sum(cart => cart.Quantity);
    }
}
