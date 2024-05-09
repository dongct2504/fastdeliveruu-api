using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class OrderDetailDto
{
    public Guid MenuItemId { get; set; }

    public Guid OrderId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public MenuItemDto MenuItemDto { get; set; } = null!;
}
