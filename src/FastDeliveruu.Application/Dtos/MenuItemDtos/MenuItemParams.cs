using FastDeliveruu.Common.Constants;

namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuItemParams : DefaultParams
{
    public Guid? GenreId { get; set; }
    public Guid? RestaurantId { get; set; }
    public new string Sort { get; set; } = SortConstants.LatestUpdateDesc;
    public Guid UserId { get; set; }

    public override string? ToString()
    {
        return $"{UserId}-{GenreId}-{RestaurantId}-{Sort}-{Search}-{PageNumber}-{PageSize}";
    }
}
