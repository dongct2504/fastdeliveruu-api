namespace FastDeliveruu.Application.Dtos.GenreDtos;

public class GenreWithMenuItemsDto
{
    public int GenreId { get; set; }

    public string Name { get; set; } = null!;

    public int RestaurantId { get; set; }

    public string MenuItemName { get; set; } = null!;

    public int Inventory { get; set; }

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public string? ImageUrl { get; set; }
}