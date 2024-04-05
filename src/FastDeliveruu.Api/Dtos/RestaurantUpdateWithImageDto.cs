using FastDeliveruu.Application.Dtos.RestaurantDtos;

namespace FastDeliveruu.Api.Dtos;

public class RestaurantUpdateWithImageDto
{
    public RestaurantUpdateDto RestaurantUpdateDto { get; set; } = null!;
    public IFormFile? ImageFile { get; set; }
}