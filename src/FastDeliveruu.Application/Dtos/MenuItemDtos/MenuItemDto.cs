namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuItemDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount => Price * DiscountPercent;

    public decimal DiscountPrice => Price - DiscountAmount;

    public string ImageUrl { get; set; } = null!;
}