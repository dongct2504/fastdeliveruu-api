using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.AddressesCustomers;

public class PrimaryAddressesCustomersByUserIdSpecification : Specification<AddressesCustomer>
{
    public PrimaryAddressesCustomersByUserIdSpecification(Guid userId)
        : base(ac => ac.AppUserId == userId && ac.IsPrimary)
    {
        
    }
}
