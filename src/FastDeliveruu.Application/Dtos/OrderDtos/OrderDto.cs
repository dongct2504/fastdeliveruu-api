using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;

namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class OrderDto
{
    public long OrderId { get; set; }

    public Guid ShipperId { get; set; }

    public Guid LocalUserId { get; set; }

    public string ReceiverName { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public string OrderDescription { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string? TrackingNumber { get; set; }

    public string? OrderStatus { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentMethod { get; set; }

    public string? TransactionId { get; set; }

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public LocalUserDto? LocalUserDto { get; set; }

    public ShipperDto? ShipperDto { get; set; }
}