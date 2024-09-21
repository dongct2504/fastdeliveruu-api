using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class MenuVariantRepository : Repository<MenuVariant>, IMenuVariantRepository
{
    public MenuVariantRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(MenuVariant menuVariant)
    {
        _dbContext.Update(menuVariant);
    }
}
