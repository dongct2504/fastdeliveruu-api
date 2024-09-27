using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Restaurants;

public class RestaurantByCityIdSpecification : Specification<Restaurant>
{
    public RestaurantByCityIdSpecification(int cityId)
        : base(r => r.CityId == cityId)
    {
    }
}
