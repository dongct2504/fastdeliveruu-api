using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommand : IRequest<Result<RestaurantDto>>
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string HouseNumber { get; set; } = null!;
    public string StreetName { get; set; } = null!;

    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }

    public IFormFile ImageFile { get; set; } = null!;
}