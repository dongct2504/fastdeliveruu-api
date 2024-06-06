using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Restaurants;

public class RestaurantByNameAndPhoneSpecification : Specification<Restaurant>
{
    public RestaurantByNameAndPhoneSpecification(string name, string phoneNumber)
        : base(r => r.Name == name && r.PhoneNumber == phoneNumber)
    {
    }
}
