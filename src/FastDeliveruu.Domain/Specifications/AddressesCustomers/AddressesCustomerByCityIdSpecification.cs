using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.AddressesCustomers;

public class AddressesCustomerByCityIdSpecification : Specification<AddressesCustomer>
{
    public AddressesCustomerByCityIdSpecification(int cityId)
        : base(ac => ac.CityId == cityId)
    {
    }
}
