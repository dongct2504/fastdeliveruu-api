using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IDeliveryMethodRepository : IRepository<DeliveryMethod>
{
    Task UpdateAsync(DeliveryMethod deliveryMethod);
}
