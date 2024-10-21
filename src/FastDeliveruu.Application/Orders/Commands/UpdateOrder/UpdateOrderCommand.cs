using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest<Result>
{
    public UpdateOrderCommand(Guid id, string paymentOrderId)
    {
        PaymentOrderId = paymentOrderId;
        Id = id;
    }

    public Guid Id { get; }
    public string PaymentOrderId { get; }
}
