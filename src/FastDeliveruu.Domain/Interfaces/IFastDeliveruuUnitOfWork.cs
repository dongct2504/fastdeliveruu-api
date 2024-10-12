namespace FastDeliveruu.Domain.Interfaces;

public interface IFastDeliveruuUnitOfWork : IDisposable
{
    IGenreRepository Genres { get; }
    IRestaurantRepository Restaurants { get; }
    IRestaurantHourRepository RestaurantHours { get; }
    IMenuItemRepository MenuItems { get; }
    IMenuVariantRepository MenuVariants { get; }
    IMenuItemInventoryRepository MenuItemInventories { get; }
    IMenuVariantInventoryRepository MenuVariantInventories { get; }
    IDeliveryMethodRepository DeliveryMethods { get; }
    IOrderRepository Orders { get; }
    IPaymentRepository Payments { get; }
    IOrderDetailRepository OrderDetails { get; }
    ICityRepository Cities { get; }
    IDistrictRepository Districts { get; }
    IWardRepository Wards { get; }
    IAddressesCustomerRepository AddressesCustomers { get; }
    IMessageThreadRepository MessageThreads { get; }
    IChatRepository Chats { get; }
    IWishListRepository WishLists { get; }

    Task SaveChangesAsync();
}
