namespace FastDeliveruu.Application.Dtos.ShoppingCartDtos;

public class CartItemDto
{
    public Guid AppUserId { get; set; }

    public Guid MenuItemId { get; set; }

    public string MenuItemName { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string GenreName { get; set; } = null!;

    public string RestaurantName { get; set; } = null!;
}
