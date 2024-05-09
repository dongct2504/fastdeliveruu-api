using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Restaurants.Commands.UpdateRestaurant;

public class UpdateRestaurantCommand : IRequest<Result>
{
    public Guid RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsVerify { get; set; }

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public IFormFile? ImageFile { get; set; }
}
