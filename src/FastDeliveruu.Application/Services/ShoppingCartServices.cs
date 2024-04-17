using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;

namespace FastDeliveruu.Application.Services;

public class ShoppingCartServices : IShoppingCartServices
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IMenuItemRepository _menuItemRepository;

    public ShoppingCartServices(IShoppingCartRepository shoppingCartRepository,
        IMenuItemRepository menuItemRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _menuItemRepository = menuItemRepository;
    }

    public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsAsync()
    {
        return await _shoppingCartRepository.ListAllAsync();
    }

    public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsByUserIdAsync(Guid userId, int page)
    {
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            SetIncludes = "MenuItem",
            PageNumber = page,
            PageSize = PagingConstants.ShoppingCartPageSize,
            Where = sc => sc.LocalUserId == userId
        };

        return await _shoppingCartRepository.ListAllAsync(options);
    }

    public async Task<Result<ShoppingCart>> GetShoppingCartByUserIdMenuItemIdAsync(Guid userId, Guid menuItemId)
    {
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            SetIncludes = "MenuItem",
            Where = sc => sc.LocalUserId == userId && sc.MenuItemId == menuItemId
        };

        ShoppingCart? shoppingCart = await _shoppingCartRepository.GetAsync(options);
        if (shoppingCart == null)
        {
            return Result.Fail<ShoppingCart>(
                new NotFoundError($"the requested shopping cart is not found."));
        }

        return shoppingCart;
    }

    public async Task<int> GetTotalShoppingCartsAsync()
    {
        return await _shoppingCartRepository.GetCountAsync();
    }

    public async Task<int> GetTotalShoppingCartsByUserIdAsync(Guid userId)
    {
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            Where = sc => sc.LocalUserId == userId
        };

        IEnumerable<ShoppingCart> shoppingCarts = await _shoppingCartRepository.ListAllAsync(options);

        return shoppingCarts.Count();
    }

    public async Task<Result> AddToShoppingCartAsync(ShoppingCart shoppingCart)
    {
        MenuItem? menuItem = await _menuItemRepository.GetAsync(shoppingCart.MenuItemId);
        if (menuItem == null)
        {
            return Result.Fail(new BadRequestError("MenuItem not found."));
        }

        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            Where = sc => sc.LocalUserId == shoppingCart.LocalUserId &&
                sc.MenuItemId == shoppingCart.MenuItemId
        };

        ShoppingCart? isExistshoppingCart = await _shoppingCartRepository.GetAsync(options);
        if (isExistshoppingCart != null) // already exist
        {
            isExistshoppingCart.Quantity += shoppingCart.Quantity;
            await _shoppingCartRepository.UpdateShoppingCart(isExistshoppingCart);

            return Result.Ok();
        }

        await _shoppingCartRepository.AddAsync(shoppingCart);

        return Result.Ok();
    }

    public async Task<Result> UpdateShoppingCartAsync(Guid menuItemId, ShoppingCart shoppingCart)
    {
        MenuItem? menuItem = await _menuItemRepository.GetAsync(shoppingCart.MenuItemId);
        if (menuItem == null)
        {
            return Result.Fail(new BadRequestError("MenuItem not found."));
        }

        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            Where = sc => sc.LocalUserId == shoppingCart.LocalUserId &&
                sc.MenuItemId == menuItemId
        };

        ShoppingCart? isExistshoppingCart = await _shoppingCartRepository.GetAsync(options);
        if (isExistshoppingCart == null)
        {
            return Result.Fail(new NotFoundError($"the requested shopping cart is not found."));
        }

        await _shoppingCartRepository.UpdateShoppingCart(shoppingCart);

        return Result.Ok();
    }

    public async Task<Result> DeleteShoppingCartAsync(Guid userId, Guid menuItemId)
    {
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            Where = sc => sc.LocalUserId == userId && sc.MenuItemId == menuItemId
        };

        ShoppingCart? isExistshoppingCart = await _shoppingCartRepository.GetAsync(options);
        if (isExistshoppingCart == null)
        {
            return Result.Fail(
                new NotFoundError($"the requested shopping cart is not found."));
        }

        await _shoppingCartRepository.UpdateShoppingCart(isExistshoppingCart);

        return Result.Ok();

    }
}