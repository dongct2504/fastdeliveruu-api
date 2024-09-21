namespace FastDeliveruu.Domain.Interfaces;

public interface IFastDeliveruuUnitOfWork : IDisposable
{
    IGenreRepository Genres { get; }
    IRestaurantRepository Restaurants { get; }
    IRestaurantHourRepository RestaurantHours { get; }
    IMenuItemRepository MenuItems { get; }
    IMenuVariantRepository MenuVariants { get; }
    IDeliveryMethodRepository DeliveryMethods { get; }
    IOrderRepository Orders { get; }

    Task SaveChangesAsync();
}
