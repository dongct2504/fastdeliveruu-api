using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Repositories;

namespace FastDeliveruu.Infrastructure.UnitOfWork;

public class FastDeliveruuUnitOfWork : IFastDeliveruuUnitOfWork
{
    private readonly FastDeliveruuDbContext _dbContext;

    public FastDeliveruuUnitOfWork(FastDeliveruuDbContext dbContext)
    {
        _dbContext = dbContext;
        Genres = new GenreRepository(_dbContext);
        Restaurants = new RestaurantRepository(_dbContext);
        RestaurantHours = new RestaurantHourRepository(_dbContext);
        MenuItems = new MenuItemRepository(_dbContext);
        MenuVariants = new MenuVariantRepository(_dbContext);
        DeliveryMethods = new DeliveryMethodRepository(_dbContext);
        Orders = new OrderRepository(_dbContext);
    }

    public IGenreRepository Genres { get; private set; }

    public IRestaurantRepository Restaurants { get; private set; }

    public IRestaurantHourRepository RestaurantHours { get; private set; }

    public IMenuItemRepository MenuItems { get; private set; }

    public IMenuVariantRepository MenuVariants { get; private set; }

    public IDeliveryMethodRepository DeliveryMethods { get; private set; }

    public IOrderRepository Orders { get; private set; }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
