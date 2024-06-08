using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.MenuItems;

public class MenuItemExistInRestaurantSpecification : Specification<MenuItem>
{
    public MenuItemExistInRestaurantSpecification(Guid restaurantId, string menuItemName)
        : base(mi => mi.Name == menuItemName && mi.RestaurantId == restaurantId)
    {
    }
}
