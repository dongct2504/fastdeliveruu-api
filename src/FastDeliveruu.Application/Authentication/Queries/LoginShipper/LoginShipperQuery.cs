using FastDeliveruu.Application.Dtos.ShipperDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Authentication.Queries.LoginShipper;

public class LoginShipperQuery : IRequest<Result<AuthenticationShipperResponse>>
{
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}