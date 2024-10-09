using FastDeliveruu.Application.Dtos.ShipperDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.ShipperLogin;

public class ShipperLoginQuery : IRequest<Result<ShipperAuthenticationResponse>>
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
