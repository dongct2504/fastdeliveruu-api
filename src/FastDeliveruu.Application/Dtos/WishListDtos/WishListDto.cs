using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Application.Dtos.WishListDtos;

public class WishListDto
{
    public Guid Id { get; set; }
    public Guid AppUserId { get; set; }
    public Guid MenuItemId { get; set; }
    public Guid? MenuVariantId { get; set; }

    public MenuItemDto MenuItemDto { get; set; } = null!;

    public MenuVariantDto? MenuVariantDto { get; set; }
}
