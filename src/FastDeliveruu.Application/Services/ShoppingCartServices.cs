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

    public ShoppingCartServices(IShoppingCartRepository shoppingCartRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
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

    public async Task<Result<ShoppingCart>> GetShoppingCartByIdAsync(int id)
    {
        ShoppingCart? shoppingCart = await _shoppingCartRepository.GetAsync(id);
        if (shoppingCart == null)
        {
            return Result.Fail<ShoppingCart>(
                new NotFoundError($"the requested shopping cart '{id}' is not found."));
        }

        return shoppingCart;
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

    public async Task<Result<int>> CreateShoppingCartAsync(ShoppingCart shoppingCart)
    {
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            Where = sc => sc.LocalUserId == shoppingCart.LocalUserId &&
                sc.MenuItemId == shoppingCart.MenuItemId
        };

        ShoppingCart? isExistshoppingCart = await _shoppingCartRepository.GetAsync(options);
        if (isExistshoppingCart != null)
        {
            return Result.Fail<int>(
                new DuplicateError($"the requested shopping cart is already exist."));
        }

        ShoppingCart createdShoppingCart = await _shoppingCartRepository.AddAsync(shoppingCart);

        return createdShoppingCart.ShoppingCartId;
    }

    public async Task<Result> UpdateShoppingCartAsync(Guid menuItemId, ShoppingCart shoppingCart)
    {
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

    public async Task<Result> DeleteShoppingCartAsync(int id)
    {
        ShoppingCart? isShoppingCartExist = await _shoppingCartRepository.GetAsync(id);
        if (isShoppingCartExist == null)
        {
            return Result.Fail(
                new NotFoundError($"the requested shopping cart is not found."));
        }

        await _shoppingCartRepository.UpdateShoppingCart(isShoppingCartExist);

        return Result.Ok();

    }
}