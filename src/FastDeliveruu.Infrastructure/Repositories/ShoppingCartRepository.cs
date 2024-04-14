using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    public ShoppingCartRepository(FastDeliveruuContext context) : base(context)
    {
    }

    public async Task UpdateShoppingCart(ShoppingCart shoppingCart)
    {
        _dbContext.Update(shoppingCart);
        await _dbContext.SaveChangesAsync();
    }
}