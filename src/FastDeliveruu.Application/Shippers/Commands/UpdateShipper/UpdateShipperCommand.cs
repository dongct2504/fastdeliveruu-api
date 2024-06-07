using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Shippers.Commands.UpdateShipper;

public class UpdateShipperCommand : IRequest<Result>
{
    public Guid ShipperId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? VehicleType { get; set; }

    public IFormFile? ImageFile { get; set; }
}