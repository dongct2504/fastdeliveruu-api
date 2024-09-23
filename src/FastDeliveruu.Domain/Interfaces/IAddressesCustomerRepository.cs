using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IAddressesCustomerRepository : IRepository<AddressesCustomer>
{
    void Update(AddressesCustomer addressesCustomer);
}
