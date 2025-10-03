using FastDeliveruu.Common.Constants;

namespace FastDeliveruu.Application.Dtos.RestaurantDtos;

public class RestaurantParams : DefaultParams
{
    public new string Sort { get; set; } = RestaurantSortConstants.LatestUpdateDesc;
}
