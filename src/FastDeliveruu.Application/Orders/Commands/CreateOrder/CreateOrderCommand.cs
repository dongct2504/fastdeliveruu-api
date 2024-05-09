using FastDeliveruu.Domain.Entities;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Result<Order>>
{
    public Guid LocalUserId { get; set; }

    public string ReceiverName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;
}
