using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.AddressesCustomers;

public class AddressesCustomerByDistrictIdSpecification : Specification<AddressesCustomer>
{
    public AddressesCustomerByDistrictIdSpecification(int districtId)
        : base(ac => ac.DistrictId == districtId)
    {
    }
}
