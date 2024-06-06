using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.MenuItems;

public class MenuItemExistInGenreOrRestaurantSpecification : Specification<MenuItem>
{
    public MenuItemExistInGenreOrRestaurantSpecification(Guid genreId, Guid restaurantId, string menuItemName)
        : base(mi => mi.Name == menuItemName && mi.GenreId == genreId ||
            mi.Name == menuItemName && mi.RestaurantId == restaurantId)
    {
    }
}
