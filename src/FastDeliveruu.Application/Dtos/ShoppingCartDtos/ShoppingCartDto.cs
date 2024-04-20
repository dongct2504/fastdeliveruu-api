using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Application.Dtos.ShoppingCartDtos;

public class ShoppingCartDto
{
    public long MenuItemId { get; set; }

    public Guid LocalUserId { get; set; }

    public int Quantity { get; set; }

    public MenuItemDto? MenuItemDto { get; set; }
}