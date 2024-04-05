namespace FastDeliveruu.Application.Dtos.RestaurantDtos;

public class RestaurantWithMenuItemsDto
{
    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsVerify { get; set; }

    public string? ImageUrl { get; set; }

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public int GenreId { get; set; }

    public string MenuItemName { get; set; } = null!;

    public int Inventory { get; set; }

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public string? MenuItemImageUrl { get; set; }
}