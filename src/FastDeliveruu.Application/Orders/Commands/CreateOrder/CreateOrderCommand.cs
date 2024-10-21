using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Result<Order>>
{
    public Guid AppUserId { get; set; }

    public int DeliveryMethodId { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public PaymentMethodsEnum PaymentMethod { get; set; }

    public string Address { get; set; } = null!;

    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }

    public string? PaymentOrderId { get; set; }
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string Reference { get; set; } = string.Empty;   
}
