namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuItemDto
{
    public int MenuItemId { get; set; }

    public int RestaurantId { get; set; }

    public int GenreId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Inventory { get; set; }

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public string? ImageUrl { get; set; }

    public string? GenreName { get; set; }

    public string? RestaurantName { get; set; }
}