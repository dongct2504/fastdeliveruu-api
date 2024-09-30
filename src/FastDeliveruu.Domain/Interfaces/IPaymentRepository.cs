using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    void Update(Payment payment);
}
