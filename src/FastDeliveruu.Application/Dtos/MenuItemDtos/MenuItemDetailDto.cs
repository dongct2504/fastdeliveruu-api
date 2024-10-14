using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;

namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuItemDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Inventory { get; set; }

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount => Price * DiscountPercent;

    public decimal DiscountPrice => Price - DiscountAmount;

    public string ImageUrl { get; set; } = null!;

    public GenreDto? GenreDto { get; set; }

    public RestaurantDto? RestaurantDto { get; set; }

    public List<MenuVariantDto> MenuVariantDtos { get; set; } = new List<MenuVariantDto>();
}