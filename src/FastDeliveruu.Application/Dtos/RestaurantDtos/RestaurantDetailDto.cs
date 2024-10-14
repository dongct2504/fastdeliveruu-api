using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Application.Dtos.RestaurantDtos;

public class RestaurantDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public IEnumerable<MenuItemDto>? MenuItemDtos { get; set; }
}