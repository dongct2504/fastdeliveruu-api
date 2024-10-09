using FastDeliveruu.Application.Dtos.ShipperDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Commands.ShipperRegister;

public class ShipperRegisterCommand : IRequest<Result<ShipperAuthenticationResponse>>
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string CitizenIdentification { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Address { get; set; } = null!;
    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }

    public string? ModelType { get; set; }
}
