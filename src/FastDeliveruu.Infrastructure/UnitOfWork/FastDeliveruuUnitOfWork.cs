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
        MenuItemInventories = new MenuItemInventoryRepository(_dbContext);
        MenuVariantInventories = new MenuVariantInventoryRepository(_dbContext);
        DeliveryMethods = new DeliveryMethodRepository(_dbContext);
        Orders = new OrderRepository(_dbContext);
        Payments = new PaymentRepository(_dbContext);
        OrderDetails = new OrderDetailRepository(_dbContext);
        Cities = new CityRepository(_dbContext);
        Districts = new DistrictRepository(_dbContext);
        Wards = new WardRepository(_dbContext);
        AddressesCustomers = new AddressesCustomerRepository(_dbContext);
        MessageThreads = new MessageThreadRepository(_dbContext);
        Chats = new ChatRepository(_dbContext);
        WishLists = new WishlistRepository(_dbContext);
        Shippers = new ShipperRepository(_dbContext);
    }

    public IGenreRepository Genres { get; private set; }

    public IRestaurantRepository Restaurants { get; private set; }

    public IRestaurantHourRepository RestaurantHours { get; private set; }

    public IMenuItemRepository MenuItems { get; private set; }

    public IMenuVariantRepository MenuVariants { get; private set; }

    public IMenuItemInventoryRepository MenuItemInventories { get; private set; }

    public IMenuVariantInventoryRepository MenuVariantInventories { get; private set; }

    public IDeliveryMethodRepository DeliveryMethods { get; private set; }

    public IOrderRepository Orders { get; private set; }

    public IPaymentRepository Payments { get; private set; }

    public IOrderDetailRepository OrderDetails { get; private set; }

    public ICityRepository Cities { get; private set; }

    public IDistrictRepository Districts { get; private set; }

    public IWardRepository Wards { get; private set; }

    public IAddressesCustomerRepository AddressesCustomers { get; private set; }

    public IWishListRepository WishLists { get; private set; }

    public IChatRepository Chats { get; private set; }

    public IMessageThreadRepository MessageThreads { get; private set; }

    public IShipperRepository Shippers { get; private set; }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
