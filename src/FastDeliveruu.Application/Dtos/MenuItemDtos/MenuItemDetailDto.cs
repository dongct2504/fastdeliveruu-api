using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;

namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuItemDetailDto
{
    public Guid MenuItemId { get; set; }

    public Guid RestaurantId { get; set; }

    public int GenreId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Inventory { get; set; }

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public string? ImageUrl { get; set; }

    public GenreDto? GenreDto { get; set; }

    public RestaurantDto? RestaurantDto { get; set; }
}