using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Restaurants;

public class RestaurantByWardIdSpecification : Specification<Restaurant>
{
    public RestaurantByWardIdSpecification(int wardId)
        : base(r => r.WardId == wardId)
    {
    }
}
