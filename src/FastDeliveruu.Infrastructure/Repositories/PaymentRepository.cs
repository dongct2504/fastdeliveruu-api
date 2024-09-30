using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(Payment payment)
    {
        _dbContext.Update(payment);
    }
}
