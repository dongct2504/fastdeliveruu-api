using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Result<Order>>
{
    public Guid AppUserId { get; set; }

    public Guid DeliveryMethodId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public PaymentMethods PaymentMethod { get; set; }

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;
}
