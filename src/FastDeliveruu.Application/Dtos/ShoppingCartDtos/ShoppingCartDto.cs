using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Application.Dtos.ShoppingCartDtos;

public class ShoppingCartDto
{
    public Guid Id { get; set; }

    public Guid MenuItemId { get; set; }

    public Guid AppUserId { get; set; }

    public Guid? MenuVariantId { get; set; }

    public int Quantity { get; set; }

    public MenuItemDto MenuItemDto { get; set; } = new MenuItemDto();

    public MenuVariantDto? MenuVariantDto { get; set; }
}