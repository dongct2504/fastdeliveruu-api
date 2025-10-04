using System;

namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class AvailableShipperOrderDto
{
    public Guid Id { get; set; }
    public DateTime? OrderDate { get; set; }

    public byte? PaymentMethod { get; set; }
    public string? PaymentMethodText { get; set; }

    public decimal TotalAmount { get; set; }

    public byte? OrderStatus { get; set; }
    public string? OrderStatusText { get; set; }

    public int? DeliveryMethodId { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}