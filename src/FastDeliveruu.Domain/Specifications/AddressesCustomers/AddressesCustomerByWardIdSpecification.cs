using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.AddressesCustomers;

public class AddressesCustomerByWardIdSpecification : Specification<AddressesCustomer>
{
    public AddressesCustomerByWardIdSpecification(int wardId)
        : base(ac => ac.WardId == wardId)
    {
    }
}
