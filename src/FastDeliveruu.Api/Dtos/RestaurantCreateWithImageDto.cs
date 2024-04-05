using FastDeliveruu.Application.Dtos.RestaurantDtos;

namespace FastDeliveruu.Api.Dtos;

public class RestaurantCreateWithImageDto
{
    public RestaurantCreateDto RestaurantCreateDto { get; set; } = null!;
    public IFormFile? ImageFile { get; set; }
}