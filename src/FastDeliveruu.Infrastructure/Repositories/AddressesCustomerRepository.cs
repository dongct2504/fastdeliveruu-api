using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class AddressesCustomerRepository : Repository<AddressesCustomer>, IAddressesCustomerRepository
{
    public AddressesCustomerRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(AddressesCustomer addressesCustomer)
    {
        _dbContext.Update(addressesCustomer);
    }
}
