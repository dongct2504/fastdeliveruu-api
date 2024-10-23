using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Restaurants.Commands.UpdateRestaurant;

public class UpdateRestaurantCommand : IRequest<Result>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsVerify { get; set; }

    public string HouseNumber { get; set; } = null!;
    public string StreetName { get; set; } = null!;

    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }

    public IFormFile? ImageFile { get; set; }
}
