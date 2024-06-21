using FastDeliveruu.Application.Common.Constants;

namespace FastDeliveruu.Application.Dtos.RestaurantDtos;

public class RestaurantParams
{
    public string Sort { get; set; } = RestaurantSortConstants.LatestUpdateDesc;

    public string Search { get; set; } = string.Empty;

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 6;

    public override string? ToString()
    {
        return $"{Sort}-{Search}-{PageNumber}-{PageSize}";
    }
}
