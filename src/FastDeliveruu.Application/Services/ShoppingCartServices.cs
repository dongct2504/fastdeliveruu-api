using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;

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

    public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsByUserIdAsync(int userId, int page)
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

    public async Task<ShoppingCart?> GetShoppingCartByIdAsync(int id)
    {
        return await _shoppingCartRepository.GetAsync(id);
    }

    public async Task<ShoppingCart?> GetShoppingCartByUserIdMenuItemIdAsync(int userId, int menuItemId)
    {
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            SetIncludes = "MenuItem",
            Where = sc => sc.LocalUserId == userId && sc.MenuItemId == menuItemId
        };

        return await _shoppingCartRepository.GetAsync(options);
    }

    public async Task<int> GetTotalShoppingCartsAsync()
    {
        return await _shoppingCartRepository.GetCountAsync();
    }

    public async Task<int> GetTotalShoppingCartsByUserIdAsync(int userId)
    {
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            Where = sc => sc.LocalUserId == userId
        };

        IEnumerable<ShoppingCart> shoppingCarts = await _shoppingCartRepository.ListAllAsync(options);

        return shoppingCarts.Count();
    }

    public async Task<int> CreateShoppingCartAsync(ShoppingCart shoppingCart)
    {
        ShoppingCart createdShoppingCart = await _shoppingCartRepository.AddAsync(shoppingCart);

        return createdShoppingCart.ShoppingCartId;
    }

    public async Task UpdateShoppingCartAsync(ShoppingCart shoppingCart)
    {
        await _shoppingCartRepository.UpdateShoppingCart(shoppingCart);
    }

    public async Task DeleteShoppingCartAsync(int id)
    {
        ShoppingCart? shoppingCart = await _shoppingCartRepository.GetAsync(id);
        if (shoppingCart != null)
        {
            await _shoppingCartRepository.DeleteAsync(shoppingCart);
        }
    }
}