using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Restaurants;

public class RestaurantByDistrictIdSpecification : Specification<Restaurant>
{
    public RestaurantByDistrictIdSpecification(int districtId)
        : base(r => r.DistrictId == districtId)
    {
    }
}
