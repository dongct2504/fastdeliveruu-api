using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IDeliveryMethodRepository : IRepository<DeliveryMethod>
{
    void Update(DeliveryMethod deliveryMethod);
}
