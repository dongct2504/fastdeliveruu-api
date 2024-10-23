using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<Result>
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string HouseNumber { get; set; } = null!;
    public string StreetName { get; set; } = null!;

    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }

    public string? Role { get; set; }

    public IFormFile? ImageFile { get; set; }
}