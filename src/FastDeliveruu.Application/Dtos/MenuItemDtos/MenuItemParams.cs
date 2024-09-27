using FastDeliveruu.Application.Common.Constants;

namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuItemParams : DefaultParams
{
    public Guid? GenreId { get; set; }

    public Guid? RestaurantId { get; set; }

    public new string Sort { get; set; } = MenuItemSortConstants.LatestUpdateDesc;

    public override string? ToString()
    {
        return $"{GenreId}-{RestaurantId}-{Sort}-{Search}-{PageNumber}-{PageSize}";
    }
}
