using System;

namespace FastDeliveruu.Application.Orders.Queries.GetShipperDeliveryHistory;

public class ShipperDeliveryHistoryDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string ShipperId { get; set; } = null!;
    public byte DeliveryStatus { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}