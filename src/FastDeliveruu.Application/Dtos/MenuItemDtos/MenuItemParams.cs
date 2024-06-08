using FastDeliveruu.Application.Common.Constants;

namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuItemParams
{
    public Guid? GenreId { get; set; }

    public Guid? RestaurantId { get; set; }

    public string Sort { get; set; } = MenuItemSortConstants.LatestUpdateDesc;

    public string Search { get; set; } = string.Empty;

    public int Page { get; set; } = 1;

    public override string? ToString()
    {
        return $"{GenreId}-{RestaurantId}-{Sort}-{Search}-{Page}";
    }
}
