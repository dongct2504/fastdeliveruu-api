namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class OrderSummaryDto
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalUnpaid { get; set; }
}
