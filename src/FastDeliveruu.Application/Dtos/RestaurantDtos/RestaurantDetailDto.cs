using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Application.Dtos.RestaurantDtos;

public class RestaurantDetailDto
{
    public Guid RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsVerify { get; set; }

    public string? ImageUrl { get; set; }

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public IEnumerable<MenuItemDto>? MenuItemDtos { get; set; }
}