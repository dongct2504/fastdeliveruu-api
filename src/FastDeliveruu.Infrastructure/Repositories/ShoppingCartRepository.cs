using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    public ShoppingCartRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public async Task UpdateAsync(ShoppingCart shoppingCart)
    {
        _dbContext.Update(shoppingCart);
        await _dbContext.SaveChangesAsync();
    }
}