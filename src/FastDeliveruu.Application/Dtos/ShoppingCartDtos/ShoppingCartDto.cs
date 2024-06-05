using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Application.Dtos.ShoppingCartDtos;

public class ShoppingCartDto
{
    public Guid LocalUserId { get; set; }

    public Guid MenuItemId { get; set; }

    public int Quantity { get; set; }

    public MenuItemDto? MenuItemDto { get; set; }
}