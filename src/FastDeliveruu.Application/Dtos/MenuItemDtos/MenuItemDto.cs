namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuItemDto
{
    public Guid MenuItemId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Inventory { get; set; }

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount => Price * DiscountPercent;

    public decimal DiscountPrice => Price - DiscountAmount;

    public string? ImageUrl { get; set; }
}